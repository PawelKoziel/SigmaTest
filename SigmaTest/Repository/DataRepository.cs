using SigmaTest.Models;
using SigmaTest.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace SigmaTest.Repository
{
    public class DataRepository : IDataRepository
    {
        private readonly IBlobAccessService _blobService;

        public DataRepository(IBlobAccessService blobService)
        {
            _blobService = blobService;
        }

        public async Task<IEnumerable<SensorBase>> GetSensorAsync(string deviceId, SensorType sensorType, DateTime date)
        {
            var requestedData = $"{deviceId}/{Enum.GetName(typeof(SensorType), sensorType)}";
            
            var forDate = $"{date:yyyy-MM-dd}.csv";

            var result = await _blobService.GetDataAsync(requestedData, forDate);

            if (result == null)
            {
                return null;
            }

            if (sensorType == SensorType.humidity)
            {
                return result.Select(x => new HumidityDto() { Date = x.Date, Humidity = x.Value }).ToList();

            }
            if (sensorType == SensorType.temperature)
            {
                return result.Select(x => new TemperatureDto() { Date = x.Date, Temperature = x.Value }).ToList();

            }
            if (sensorType == SensorType.rainfall)
            {
                return result.Select(x => new RainfallDto() { Date = x.Date, Rainfall = x.Value }).ToList();

            }
            return null;
        }

        public async Task<IEnumerable<AllSensorData>> GetAllSensorAsync(string deviceId, DateTime date)
        {
            var requestedBlob = $"{date:yyyy-MM-dd}.csv";
            
            var blobpath = $"{deviceId}/humidity";
            var humidity = await _blobService.GetDataAsync(blobpath,requestedBlob);
            if (humidity == null) 
                return null;

            blobpath = $"{deviceId}/rainfall";
            var rainfall = await _blobService.GetDataAsync(blobpath, requestedBlob);
            if (humidity == null)
                return null;

            blobpath = $"{deviceId}/temperature";
            var temperature = await _blobService.GetDataAsync(blobpath, requestedBlob);
            if (humidity == null)
                return null;

            List<AllSensorData> result = new List<AllSensorData>();
            for (int i = 0; i < humidity.Count(); i++)
            {
                result.Add(new AllSensorData(humidity[i],temperature[i],rainfall[i]));
            }


            //łączenie na podstawie daty
            //var listab = from h1 in humidity
            //             join r1 in rainfall on h1.Date equals r1.Date
            //             select new AllSensorData() { Date = h1.Date, Humidity = h1.Value, Rainfall = r1.Value };


            //var result = from s1 in listab
            //             join t1 in temperature on s1.Date equals t1.Date
            //             select new AllSensorData() { Date = s1.Date, Humidity = s1.Humidity, Rainfall = s1.Rainfall, Temperature = t1.Value };

             return result.ToList();
        }

    }

}
