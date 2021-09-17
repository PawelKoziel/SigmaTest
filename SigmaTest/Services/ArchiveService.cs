using Azure.Storage.Blobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SigmaTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SigmaTest.Services
{
    public class ArchiveService : IArchiveService
    {

        private readonly BlobContainerClient container;
        private readonly ILogger<ArchiveService> _logger;
        private readonly ICsvParsingService _parsingService;

        public ArchiveService(ILogger<ArchiveService> logger, IAzureConnector connector, ICsvParsingService parsingService)
        {
            _logger = logger;
            _parsingService = parsingService;
            container = connector.GetContainer();
            
        }

        public async Task<List<DataPoint>> GetData(string blobPath, string blobName)
        {
            var archiveName = await GetArchive(blobPath, blobName);
            var data = ExtractDataFromArchive(blobName, archiveName);
            
            return data;
        }


        public async Task<string> GetArchive(string blobPath, string blobName)
        {

            var archiveName = blobPath + "/historical.zip";
            var blob = container.GetBlobClient(archiveName);

            try
            {

                Directory.CreateDirectory(Path.Combine("Cache", blobPath));
                var localArchiveName = Path.Combine("Cache", blobPath, "historical.zip");

                if (File.Exists(localArchiveName))
                {
                    var blobProperties = await blob.GetPropertiesAsync();
                    var blobModifDate = blobProperties.Value.LastModified.DateTime;
                    var fileModifDate = File.GetLastWriteTime(localArchiveName);

                    if (blobModifDate > fileModifDate)
                    {
                        File.Delete(localArchiveName);
                        await blob.DownloadToAsync(localArchiveName);
                    }
                }
                else
                {
                    await blob.DownloadToAsync(localArchiveName);
                }
                return localArchiveName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling archive file for path {blobPath}");
                throw;
            }
        }


        private List<DataPoint> ExtractDataFromArchive(string blobName, string archiveFile)
        {
            try
            {
                using (var archiveStream = new MemoryStream())
                {
                    using (FileStream file = new FileStream(archiveFile, FileMode.Open, FileAccess.Read))
                        file.CopyTo(archiveStream);

                    ZipArchive archive = new ZipArchive(archiveStream);
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (entry.FullName.EndsWith(blobName, StringComparison.OrdinalIgnoreCase))
                        {
                            using (Stream unzippedEntryStream = entry.Open())
                            using (var dataStream = new MemoryStream())
                            {
                                unzippedEntryStream.CopyTo(dataStream);
                                return _parsingService.ParseCsvData(dataStream);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching {blobName} in archive file {archiveFile}");
            }
            return null;
        }
    }
}
