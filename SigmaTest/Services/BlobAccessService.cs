using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Logging;
using SigmaTest.Models;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SigmaTest.Services
{
    public class BlobAccessService : IBlobAccessService
    {
        private readonly IAzureConnector _connector;
        private readonly ICsvParsingService _parsingService;
        private readonly ILogger<BlobAccessService> _logger;

        private readonly BlobContainerClient container;

        public BlobAccessService(ILogger<BlobAccessService> logger
                                , IAzureConnector connector
                                , ICsvParsingService parsingService)
        {
            _logger = logger;
            _connector = connector;
            _parsingService = parsingService;

            container = _connector.GetContainer();
        }


        public async Task<IEnumerable<DataPoint>> GetDataAsync(string blobPath, string blobName)
        {
            var blob = container.GetBlobClient(blobPath + "/" + blobName);

            if (await blob.ExistsAsync())
            {
                using var dataStream = new MemoryStream();
                await blob.DownloadToAsync(dataStream);
                return _parsingService.ParseCsvData(dataStream);
            }
            else
            {
                var localArchiveName = await HandleDataArchive(blobName, blobPath);
                return GetDataFromArchive(blobName, localArchiveName);
            }
        }


        private async Task<string> HandleDataArchive(string blobName, string blobPath)
        {

            var archiveName = blobPath + "/historical.zip";
            var blob = container.GetBlobClient(archiveName);

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


        public IEnumerable<DataPoint> GetDataFromArchive(string blobName, string archiveFile)
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
            return null;
        }

    }
}
