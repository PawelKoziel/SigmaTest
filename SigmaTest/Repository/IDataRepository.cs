using Microsoft.AspNetCore.Mvc;
using SigmaTest.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SigmaTest.Repository
{
    public interface IDataRepository
    {
        Task<IActionResult> GetAllSensorAsync(string deviceId, DateTime date);
        Task<IActionResult> GetSensorAsync(string deviceId, SensorType sensorType, DateTime date);
    }
}