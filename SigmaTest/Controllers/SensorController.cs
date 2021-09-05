using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SigmaTest.Models;
using SigmaTest.Repository;
using System;
using System.Threading.Tasks;

namespace SigmaTest.Controllers
{
    [ApiController]
    //[Route("/api/v1")]
    [Route("/")]
    public class SensorController : ControllerBase
    {
        private readonly ILogger<SensorController> _logger;
        private readonly IDataRepository _repository;

        public SensorController(ILogger<SensorController> logger, IDataRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        //api/v1/dockan/data/2019-01-11
        [HttpGet("api/v1/{deviceId}/data/{date}")]
        public async Task<IActionResult> GetRoute(string deviceId, DateTime date)
        {
            if (String.IsNullOrEmpty(deviceId) || date == default)
            {
                return BadRequest();
            }

            var result = await _repository.GetAllSensorAsync(deviceId, date);


            if (result == null)
            {
                return NotFound();
            }

            return new JsonResult(result);
        }


        //	getdatafordevice?deviceId=dockan&date=2019-01-11
        [HttpGet("getdatafordevice")]
        public async Task<IActionResult> GetQuery([FromQuery]string deviceId, [FromQuery] DateTime date)
        {
            if (String.IsNullOrEmpty(deviceId) || date == default)
            {
                return BadRequest();
            }

            var result = await _repository.GetAllSensorAsync(deviceId, date);


            if (result == null)
            {
                return NotFound();
            }

            return new JsonResult(result);
        }


        //api/v1/dockan/data/2019-01-11
        [HttpGet("api/v1/{deviceId}/data/{date}/{sensorType}")]
        public async Task<IActionResult> GetRoute(string deviceId, DateTime date, SensorType sensorType)
        {
            if (String.IsNullOrEmpty(deviceId) || date == default)
            {
                return BadRequest();
            }

            var result = await _repository.GetSensorAsync(deviceId, sensorType, date);


            if (result == null)
            {
                return NotFound();
            }
            
            return new JsonResult(result);
        }
    }
}
