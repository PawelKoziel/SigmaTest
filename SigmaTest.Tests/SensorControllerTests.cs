using System;
using System.Collections.Generic;
using Xunit;
using System.Text;
using SigmaTest.Controllers;
using Moq;
using SigmaTest.Repository;
using Microsoft.AspNetCore.Mvc;
using SigmaTest.Models;

namespace SigmaTest.Tests
{
    public class SensorControllerTests
    {
        [Fact]
        public async void Empty_parameters_returns_BadRequest()
        {
            var mockRepo = new Mock<IDataRepository>();          
            var controller = new SensorController(mockRepo.Object);

            var result = await controller.GetDeviceDataQuery(string.Empty, new DateTime());

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async void If_sensor_not_found_returns_notFoundResult()
        {
            var mockRepo = new Mock<IDataRepository>();
            mockRepo.Setup(r => r.GetSensorAsync(It.IsAny<string>(), It.IsAny<SensorType>(), It.IsAny<DateTime>())).ReturnsAsync(value: null);
            var controller = new SensorController(mockRepo.Object);

            var result = await controller.GetSensorDataQuery("sensor1", DateTime.Now, SensorType.humidity);

            Assert.IsType<NotFoundResult>(result);
        }

    }
}
