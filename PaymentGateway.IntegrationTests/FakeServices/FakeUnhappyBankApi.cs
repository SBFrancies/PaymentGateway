using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.BankApi;
using System.Threading.Tasks;

namespace PaymentGateway.IntegrationTests.FakeServices
{
    public class FakeUnhappyBankApi : IBankApi
    {
        public Task<bool> CheckBankHealthyAsync()
        {
            return Task.FromResult(true);
        }

        public Task<CardPaymentResponse> MakeBankPaymentAsync(CardPaymentRequest request)
        {
            var response = new CardPaymentResponse
            {
                DateProcessed = null,
                Message = "Something bad happened",
                Success = false,
            };

            return Task.FromResult(response);
        }
    }
}
