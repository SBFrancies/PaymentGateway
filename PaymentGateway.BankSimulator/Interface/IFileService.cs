using System.Threading.Tasks;

namespace PaymentGateway.BankSimulator.Interface
{
    public interface IFileService
    {
        Task<string[]> ReadAllLinesAsync(string filePath);
    }
}
