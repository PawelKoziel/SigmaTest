using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SigmaTest.Models;
using SigmaTest.Repository;
using SigmaTest.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace SigmaTest.Tests
{
    public class DataRepositoryTests
    {

        Mock<IDataAccessService> connector = new Moq.Mock<IDataAccessService>(MockBehavior.Loose);
        ILogger<DataRepository> logger = Mock.Of<ILogger<DataRepository>>();

        [Fact]
        private async void Should_Pass_Parameters_To_GetDataAsync()
        {
            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>()));
            DataRepository dr = new DataRepository(logger, connector.Object);

            await dr.GetSensorAsync("sensor1", SensorType.humidity, new DateTime(2011,11,11));

            connector.Verify(m => m.GetDataAsync("sensor1/humidity", "2011-11-11.csv"), Times.Once);
        }


        [Fact]
        private async void Should_Return_NotFound_If_No_Data()
        {
            List<DataPoint> dataPoints = null;

            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dataPoints);
            DataRepository dr = new DataRepository(logger, connector.Object);

            var result = await dr.GetSensorAsync("sensor1", SensorType.humidity, DateTime.Now);

            Assert.IsType<NotFoundResult>(result);
        }


        [Fact]
        private async void Should_Return_Some_Result()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dataPoints);
            DataRepository repo = new DataRepository(logger, connector.Object);

            var result = await repo.GetSensorAsync("sensor1", SensorType.humidity, DateTime.Now);

            Assert.IsType<JsonResult>(result);
        }


        [Fact]
        public async void Should_Return_BadRequest_For_No_Sensor()
        {
            List<DataPoint> dataPoints = new List<DataPoint>() 
                { new DataPoint() { Date= new DateTime(2011,11,11), Value = 11.11f },
                  new DataPoint() { Date= new DateTime(2011,11,12), Value = 13.13f },
                  new DataPoint() { Date= new DateTime(2011,11,13), Value = 13.13f }};

            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dataPoints);
            DataRepository repo = new DataRepository(logger, connector.Object);

                      
            var result = await repo.GetAllSensorAsync("", DateTime.Now);

            Assert.IsType<BadRequestResult>(result);
        }
    }
}
