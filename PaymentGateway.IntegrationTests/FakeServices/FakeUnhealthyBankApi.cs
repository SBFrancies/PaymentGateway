using PaymentGateway.Api.Interface;
using PaymentGateway.Api.Models.BankApi;
using System;
using System.Threading.Tasks;

namespace PaymentGateway.IntegrationTests.FakeServices
{
    public class FakeUnhealthyBankApi : IBankApi
    {
        public Task<bool> CheckBankHealthyAsync()
        {
            return Task.FromResult(false);
        }

        public Task<CardPaymentResponse> MakeBankPaymentAsync(CardPaymentRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
