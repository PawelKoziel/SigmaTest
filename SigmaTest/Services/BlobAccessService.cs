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
        private readonly IConfiguration _configuration;
        private readonly ICsvParsingService _parsingService;
        private readonly ILogger<BlobAccessService> _logger;

        public BlobAccessService(ILogger<BlobAccessService> logger
                                , IConfiguration configuration
                                , ICsvParsingService parsingService)
        {
            _logger = logger;
            _configuration = configuration;
            _parsingService = parsingService;
        }


        public async Task<IEnumerable<DataPoint>> GetDataAsync(string blobPath, string blobName)
        {
            var client = new BlobServiceClient(_configuration.GetConnectionString("Azure"));
            var container = client.GetBlobContainerClient(_configuration.GetValue<string>("BlobContainer"));

            IEnumerable<DataPoint> result = new List<DataPoint>();

            try
            {
                using (var dataStream = new MemoryStream())
                {
                    var blob = container.GetBlobClient(blobPath + "/" + blobName);

                    await blob.DownloadToAsync(dataStream);
                    return _parsingService.ParseCsvData(dataStream);
                }
            }
            catch (RequestFailedException) //RequestFailedException
            {
                var localArchiveName = await ArchiveHandlerAsync(blobName, blobPath, container);
                return GetDataFromArchive(blobName, localArchiveName);
            }
        }


        private async Task<string> ArchiveHandlerAsync(string blobName, string blobPath, BlobContainerClient containerClient)
        {

            var archiveName = blobPath + "/historical.zip";
            var blob = containerClient.GetBlobClient(archiveName);

            Directory.CreateDirectory(Path.Combine("Cache", blobPath));

            var localArchiveName = System.IO.Path.Combine("Cache", blobPath, "historical.zip");

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


        private IEnumerable<DataPoint> GetDataFromArchive(string blobName, string localArchiveName)
        {
            using (var archiveStream = new MemoryStream())
            {
                using (FileStream file = new FileStream(localArchiveName, FileMode.Open, FileAccess.Read))
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
