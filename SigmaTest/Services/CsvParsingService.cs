using SigmaTest.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using TinyCsvParser;
using TinyCsvParser.Mapping;
using TinyCsvParser.TypeConverter;

namespace SigmaTest.Services
{
    public class CsvParsingService : ICsvParsingService
    {
        public List<DataPoint> ParseCsvData(Stream stream)
        {
            stream.Position = 0;
            var csvParserOptions = new CsvParserOptions(false, ';'); 
            var csvMapper = new CsvSensorMapping();
            CsvParser<DataPoint> csvParser = new CsvParser<DataPoint>(csvParserOptions, csvMapper);
            var result = csvParser
                         .ReadFromStream(stream, encoding: Encoding.Default)
                         .Select(x => x.Result)
                         .ToList();

            return result;
        }

        private class CsvSensorMapping : CsvMapping<DataPoint>
        {
            public CsvSensorMapping() : base()
            {
                MapProperty(0, x => x.Date);
                MapProperty(1, x => x.Value, new SingleConverter(CultureInfo.CurrentCulture));
            }
        }
    }
}
