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

            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dataPoints);
            DataRepository dr = new DataRepository(connector.Object);

            var result = await dr.GetSensorAsync("sensor1", SensorType.humidity, DateTime.Now);

            Assert.Null(result);
        }


        [Fact]
        private async void Should_Return_Humidity()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();

            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dataPoints);
            DataRepository repo = new DataRepository(connector.Object);

            var result = await repo.GetSensorAsync("sensor1", SensorType.humidity, DateTime.Now);

            Assert.IsType<List<HumidityDto>>(result);
        }


        [Fact]
        public async void Test2()
        {
            List<DataPoint> dataPoints = new List<DataPoint>() 
                { new DataPoint() { Date= new DateTime(2011,11,11), Value = 11.11f },
                  new DataPoint() { Date= new DateTime(2011,11,12), Value = 13.13f },
                  new DataPoint() { Date= new DateTime(2011,11,13), Value = 13.13f }};


            connector.Setup(c => c.GetDataAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(dataPoints);

            DataRepository repo = new DataRepository(connector.Object);

            var result = await repo.GetAllSensorAsync("", DateTime.Now);


            Assert.Equal(3, result.Count());



        }
    }
}
