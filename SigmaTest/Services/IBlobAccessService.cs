using SigmaTest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SigmaTest.Services
{
    public interface IBlobAccessService
    {
        Task<IEnumerable<DataPoint>> GetDataAsync(string blobPath, string blobName);
    }
}