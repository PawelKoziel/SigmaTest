using SigmaTest.Models;
using System.Collections.Generic;
using System.IO;

namespace SigmaTest.Services
{
    public interface ICsvParsingService
    {
        List<DataPoint> ParseCsvData(Stream stream);
    }
}