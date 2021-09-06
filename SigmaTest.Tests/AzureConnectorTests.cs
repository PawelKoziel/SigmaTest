using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SigmaTest.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SigmaTest.Tests
{
    public class AzureConnectorTests
    {

        [Fact]
        public void Should_Throw_If_Not_Configured()
        {
            ILogger<AzureConnector> logger = Mock.Of<ILogger<AzureConnector>>();

            Mock<IConfiguration> mockConfig = new Moq.Mock<IConfiguration>(MockBehavior.Loose);
            mockConfig.Setup(a => a.GetSection(It.IsAny<string>())).Returns(value: null) ;

            AzureConnector ac = new AzureConnector(mockConfig.Object, logger);

            Assert.Throws<Exception>(() =>ac.GetContainer());
        }
    }
}
