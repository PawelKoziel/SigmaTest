using SigmaTest.Models;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace SigmaTest.Services
{
    public interface IDataAccessService
    {
        Task<List<DataPoint>> GetDataAsync(string blobPath, string blobName);
    }
}