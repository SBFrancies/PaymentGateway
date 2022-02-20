using PaymentGateway.BankSimulator.Interface;
using System.IO;
using System.Threading.Tasks;

namespace PaymentGateway.BankSimulator.Service
{
    public class FileService : IFileService
    {
        public async Task<string[]> ReadAllLinesAsync(string filePath)
        {
            return await File.ReadAllLinesAsync(filePath);
        }
    }
}
