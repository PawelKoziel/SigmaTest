using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<DataRepository> _logger;
        private readonly IDataAccessService _blobService;

        public DataRepository(ILogger<DataRepository> logger, IDataAccessService blobService)
        {
            _logger = logger;
            _blobService = blobService;
        }

        public async Task<IActionResult> GetSensorAsync(string deviceId, SensorType sensorType, DateTime date)
        {
            if (String.IsNullOrEmpty(deviceId))
            {
                return new BadRequestResult();
            }

            var requestedData = $"{deviceId}/{Enum.GetName(typeof(SensorType), sensorType)}";
            var forDate = $"{date:yyyy-MM-dd}.csv";

            try
            {
                var result = await _blobService.GetDataAsync(requestedData, forDate);

                if (result == null)
                {
                    return new NotFoundResult();
                }

                switch (sensorType)
                {
                    case SensorType.temperature:
                        return new JsonResult(result.Select(x => new TemperatureDto() { Date = x.Date, Temperature = (float)x.Value }).ToList());
                    case SensorType.rainfall:
                        return new JsonResult(result.Select(x => new RainfallDto() { Date = x.Date, Rainfall = (float)x.Value }).ToList());
                    case SensorType.humidity:
                        return new JsonResult(result.Select(x => new HumidityDto() { Date = x.Date, Humidity = (float)x.Value }).ToList());
                    default:
                        return new NotFoundResult();
                }

            }
            catch (RequestFailedException ex)
            {
                if (ex.ErrorCode == "BlobNotFound")
                {
                    return new NotFoundResult();
                }
                return new StatusCodeResult(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting data", ex);
                return new StatusCodeResult(500);
            }
        }


        public async Task<IActionResult> GetAllSensorAsync(string deviceId, DateTime date)
        {
            if (String.IsNullOrEmpty(deviceId))
            {
                return new BadRequestResult();
            }

            var requestedBlob = $"{date:yyyy-MM-dd}.csv";

            try
            {
                var humidityTask = _blobService.GetDataAsync($"{deviceId}/humidity", requestedBlob);
                var rainfallTask = _blobService.GetDataAsync($"{deviceId}/rainfall", requestedBlob);
                var temperatTask = _blobService.GetDataAsync($"{deviceId}/temperature", requestedBlob);


                await Task.WhenAll(humidityTask, rainfallTask, temperatTask);

                var humidity = await humidityTask;
                var temperature = await temperatTask;
                var rainfall = await rainfallTask;

                if (humidity == null || temperature == null || rainfall == null)
                {
                    return new NotFoundResult();
                }

                var sensorData = CombineSensorData(humidity, temperature, rainfall);

                return new JsonResult(sensorData);
            }

            catch (RequestFailedException ex)
            {
                if (ex.ErrorCode == "BlobNotFound")
                {
                    return new NotFoundResult();
                }
                return new StatusCodeResult(500);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting data",ex);
                return new StatusCodeResult(500);
            }
        }


        private List<AllSensorData> CombineSensorData(List<DataPoint> humidity, List<DataPoint> temperature, List<DataPoint> rainfall)
        {           
            var longestList = new[] { humidity.Count, temperature.Count, rainfall.Count }.Max();

            List<AllSensorData> result = new List<AllSensorData>();
            for (int i = 0; i < longestList; i++)
            {
                var humValue = humidity.Count > i ? humidity[i] : null;
                var tempValue = temperature.Count > i ? temperature[i] : null;
                var rainValue = rainfall.Count > i ? rainfall[i] : null;

                result.Add(new AllSensorData(humValue, tempValue, rainValue));
            }

            return result;
        }
    }

}
