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
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SigmaTest.Tests")]
namespace SigmaTest.Services
{
    public class DataAccessService : IDataAccessService
    {
        private readonly IAzureConnector _connector;
        private readonly ICsvParsingService _parsingService;
        private readonly IArchiveService _archive;
        private readonly ILogger<DataAccessService> _logger;
        private readonly BlobContainerClient container;


        public DataAccessService(ILogger<DataAccessService> logger
                                , IAzureConnector connector
                                , ICsvParsingService parsingService
                                , IArchiveService archive)
        {
            _logger = logger;
            _connector = connector;
            _parsingService = parsingService;
            _archive = archive;
            container = _connector.GetContainer();
        }


        public async Task<List<DataPoint>> GetDataAsync(string blobPath, string blobName)
        {
            if (String.IsNullOrEmpty(blobPath) || String.IsNullOrEmpty(blobName))
            {
                return null;
            }

            var blob = container.GetBlobClient(blobPath + "/" + blobName);

            try
            {
                if (await blob.ExistsAsync())
                {
                    using var dataStream = new MemoryStream();
                    await blob.DownloadToAsync(dataStream);
                    return _parsingService.ParseCsvData(dataStream);
                }
                else
                {
                    return await _archive.GetData(blobPath, blobName);                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting data");
                throw;
            }
        }
    }
}
