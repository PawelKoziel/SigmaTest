using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaTest.Models
{
    public class AllSensorData : SensorBase
    {
        public AllSensorData()
        {

        }
        public AllSensorData(DataPoint humidity, DataPoint temperature, DataPoint rainfall)
        {
            this.Date = temperature.Date;
            this.Humidity = humidity.Value;
            this.Temperature = temperature.Value;
            this.Rainfall = rainfall.Value;
        }

        public Single Temperature { get; set; }

        public Single Humidity { get; set; }

        public Single Rainfall { get; set; }
    }
}
