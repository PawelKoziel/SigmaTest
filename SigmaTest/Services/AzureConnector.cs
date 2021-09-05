using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaTest.Services
{
    public class AzureConnector : IAzureConnector
    {
        private readonly IConfiguration _configuration;
        public AzureConnector(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public BlobContainerClient GetContainer()
        {
            try
            {
                var client = new BlobServiceClient(_configuration.GetConnectionString("Azure"));
                var container = client.GetBlobContainerClient(_configuration.GetValue<string>("BlobContainer"));

                return container;
            }
            catch
            {
                throw;
            }
        }
    }
}
