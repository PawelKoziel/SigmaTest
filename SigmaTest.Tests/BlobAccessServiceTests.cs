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
        ILogger<DataAccessService> logger = Mock.Of<ILogger<DataAccessService>>();

        [Fact]
        public async void Dont_use_empty_parameters()
        {                     
            Mock<IAzureConnector> connector = new Moq.Mock<IAzureConnector>();
            Mock<IArchiveService> archive = new Moq.Mock<IArchiveService>();
            var parser = Mock.Of<ICsvParsingService>();
            var blobService = new DataAccessService(logger, connector.Object, parser, archive.Object);

            var result = await blobService.GetDataAsync("", "");

            Assert.Null(result);
        }

    }
}
