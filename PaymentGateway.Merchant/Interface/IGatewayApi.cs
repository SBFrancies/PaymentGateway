using PaymentGateway.Merchant.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Interface
{
    public interface IGatewayApi
    {
        Task<PaymentResponse> GetAsync(Guid id);

        Task<NewPaymentResponse> PostAsync(CreatePayment createPayment);

        Task<IEnumerable<Payment>> GetAsync(string cardNumber, string reference);
    }
}
