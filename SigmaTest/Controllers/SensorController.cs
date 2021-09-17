using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SigmaTest.Models;
using SigmaTest.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SigmaTest.Controllers
{
    [ApiController]
    [Route("/")]
    public class SensorController : ControllerBase
    {
        private readonly IDataRepository _repository;
        public SensorController(IDataRepository repository)
        {
            _repository = repository;
        }

        //api/v1/devices/dockan/data/2019-01-11
        [HttpGet("api/v1/devices/{deviceId}/data/{date}")]
        public async Task<IActionResult> GetDeviceDataRoute(string deviceId, DateTime date)
        {
            var result = await _repository.GetAllSensorAsync(deviceId, date);
            return result;

        }


        //getdatafordevice?deviceId=dockan&date=2018-09-18
        [HttpGet("getdatafordevice")]
        public async Task<IActionResult> GetDeviceDataQuery([FromQuery] string deviceId, [FromQuery] DateTime date)
        {
            var result = await _repository.GetAllSensorAsync(deviceId, date);
            return result;
        }


        //api/v1/devices/dockan/data/2019-01-11
        [HttpGet("api/v1/devices/{deviceId}/data/{date}/{sensorType}")]
        public async Task<IActionResult> GetSensorDataRoute(string deviceId, DateTime date, SensorType sensorType)
        {
            var result = await _repository.GetSensorAsync(deviceId, sensorType, date);
            return result;
        }



        //getdata?deviceId=dockan&date=2016-09-18&sensorType=rainfall
        [HttpGet("getdata")]

        public async Task<IActionResult> GetSensorDataQuery([FromQuery] string deviceId,
                                                            [FromQuery] DateTime date,
                                                            [FromQuery] SensorType sensorType)
        {
            var result = await _repository.GetSensorAsync(deviceId, sensorType, date);
            return result;

        }

   }
}
