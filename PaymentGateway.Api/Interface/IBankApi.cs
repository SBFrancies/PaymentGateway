using PaymentGateway.Api.Models.BankApi;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Interface
{
    public interface IBankApi
    {
        Task<CardPaymentResponse> MakeBankPaymentAsync(CardPaymentRequest request);

        Task<bool> CheckBankHealthyAsync();
    }
}
