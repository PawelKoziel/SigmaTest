using Azure.Storage.Blobs;

namespace SigmaTest.Services
{
    public interface IAzureConnector
    {
        BlobContainerClient GetContainer();
    }
}