using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using SigmaTest.Services;
using Moq;
using Microsoft.Extensions.Logging;

namespace SigmaTest.Tests
{
    public class BlobAccessServiceTests
    {
        ILogger<BlobAccessService> logger = Mock.Of<ILogger<BlobAccessService>>();

        [Fact]
        public async void Dont_use_empty_parameters()
        {                     
            Mock<IAzureConnector> connector = new Moq.Mock<IAzureConnector>();
            var parser = Mock.Of<ICsvParsingService>();
            var blobService = new BlobAccessService(logger, connector.Object, parser);

            var result = await blobService.GetDataAsync("", "");

            Assert.Null(result);
        }

    }
}
