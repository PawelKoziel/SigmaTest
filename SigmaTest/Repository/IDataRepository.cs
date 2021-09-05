using SigmaTest.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SigmaTest.Repository
{
    public interface IDataRepository
    {
        Task<IEnumerable<AllSensorData>> GetAllSensorAsync(string deviceId, DateTime date);
        Task<IEnumerable<SensorBase>> GetSensorAsync(string deviceId, SensorType sensorType, DateTime date);
    }
}