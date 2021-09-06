using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaTest.Services
{
    public class AzureConnector : IAzureConnector
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AzureConnector> _logger;

        public AzureConnector(IConfiguration configuration, ILogger<AzureConnector> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public BlobContainerClient GetContainer()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString("Azure") 
                            ?? throw new Exception("Azure ConnectionString configuration is missing!");
                var confContainer = _configuration.GetValue<string>("BlobContainer") 
                            ?? throw new Exception("BlobContainer configuration is missing!");

                var client = new BlobServiceClient(connectionString);
                var container = client.GetBlobContainerClient(confContainer);

                return container;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in Azure connection");
                throw;
            }
        }
    }
}
