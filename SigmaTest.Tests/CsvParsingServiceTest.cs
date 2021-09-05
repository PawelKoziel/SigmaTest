using SigmaTest.Models;
using SigmaTest.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace SigmaTest.Tests
{
    public class CsvParsingServiceTest
    {
        public MemoryStream CreateTestStream(string value)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(value ?? ""),false);
        }


        [Fact]
        public void Should_Find_Proper_Number_Of_DataPoints()
        {
            CsvParsingService service = new CsvParsingService();
            var dataStream = CreateTestStream("2010-11-01T00:00:00;37,01\r\n2010 - 11 - 01T00: 00:05; 37,01\r\n2010 - 11 - 01T00: 00:10; 37,00");
            var expectedCount = 3;

            var actual = service.ParseCsvData(dataStream);

            Assert.Equal(expectedCount, actual.Count);
        }



        [Fact]
        public void Should_Properly_Map_DataPoint()
        {
            CsvParsingService service = new CsvParsingService();
            var dataStream = CreateTestStream("2010-11-01T00:00:00;37,01\r\n2010 - 11 - 01T00: 00:05; 37,01\r\n2010 - 11 - 01T00: 00:10; 37,00");
            var expected = new DataPoint() { Date = new DateTime(2010, 11, 1, 0, 0, 5), Value = 37.01F }; 

            var actual = service.ParseCsvData(dataStream);

            Assert.Equal(actual[1].Date, expected.Date);
            Assert.Equal(actual[1].Value, expected.Value);
        }
    }
}
