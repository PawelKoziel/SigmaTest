using SigmaTest.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SigmaTest.Services
{
    public interface IArchiveService
    {
        Task<string> GetArchive(string blobPath, string blobName);

        Task<List<DataPoint>> GetData(string blobPath, string blobName);
    }
}