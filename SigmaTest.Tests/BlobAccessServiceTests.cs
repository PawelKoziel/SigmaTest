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
        public void Test1()
        {          
            
            Mock<IAzureConnector> connector = new Moq.Mock<IAzureConnector>();
            //connector.Setup(x=>x.GetContainer()).Returns()

            var parser = Mock.Of<ICsvParsingService>();



            var blobService = new BlobAccessService(logger, connector.Object, parser);


        }

    }
}
