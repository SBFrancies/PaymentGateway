using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.BankApi;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.IntegrationTests.FakeServices
{
    public class FakeHappyBankApi : IBankApi
    {
        public Task<bool> CheckBankHealthyAsync()
        {
            return Task.FromResult(true);
        }

        public Task<CardPaymentResponse> MakeBankPaymentAsync(CardPaymentRequest request)
        {
            var response = new CardPaymentResponse
            {
                DateProcessed = DateTime.UtcNow,
                Message = Guid.NewGuid().ToString(),
                Success = true,
            };

            return Task.FromResult(response);
        }
    }
}
