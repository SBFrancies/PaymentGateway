using PaymentGateway.BankSimulator.Models;
using System.Threading.Tasks;

namespace PaymentGateway.BankSimulator.Interface
{
    public interface IClientRepository
    {
        Task<BankClient> GetClientAsync(string apiKey);
    }
}
