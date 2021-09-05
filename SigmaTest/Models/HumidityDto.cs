using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SigmaTest.Models
{
    public class HumidityDto : SensorBase
    {
        //public DateTime Date { get; set; }

        public Single Humidity { get; set; }
    }
}
