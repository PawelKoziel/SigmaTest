using Moq;
using SigmaTest.Models;
using SigmaTest.Repository;
using SigmaTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigmaTest.Tests
{
    public class DataRepositoryTests
    {

        Mock<IBlobAccessService> connector = new Moq.Mock<IBlobAccessService>(MockBehavior.Loose);

        [Fact]
        private async void Should_Pass_Parameters_To_GetDataAsync()
        {
            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>()));
            DataRepository dr = new DataRepository(connector.Object);

            await dr.GetSensorAsync("sensor1", SensorType.humidity, new DateTime(2011,11,11));

            connector.Verify(m => m.GetDataAsync("sensor1/humidity", "2011-11-11.csv"), Times.Once);
        }

        [Fact]
        private async void Should_Return_Null_If_No_Data()
        {
            List<DataPoint> dataPoints = null;

            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(dataPoints.AsEnumerable()));
            DataRepository dr = new DataRepository(connector.Object);

            var result = await dr.GetSensorAsync("sensor1", SensorType.humidity, DateTime.Now);

            Assert.Null(result);
        }

        [Fact]
        private async void Should_Return_Null_If_N()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(dataPoints.AsEnumerable()));
            DataRepository dr = new DataRepository(connector.Object);

            var result = await dr.GetSensorAsync("sensor1", SensorType.humidity, DateTime.Now);

            Assert.IsType<List<HumidityDto>>(result);
        }

    }
}
