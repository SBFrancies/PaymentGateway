using PaymentGateway.BankSimulator.Models.Request;
using PaymentGateway.BankSimulator.Models.Response;
using System.Net;
using System.Threading.Tasks;

namespace PaymentGateway.BankSimulator.Interface
{
    public interface IPaymentProcessor
    {
        Task<(HttpStatusCode code, CardPaymentResponse response)> ProcessPaymentAsync(CardPaymentRequest paymentRequest);
    }
}
