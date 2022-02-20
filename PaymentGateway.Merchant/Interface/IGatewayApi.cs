using PaymentGateway.Merchant.Models.ApiModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Interface
{
    public interface IGatewayApi
    {
        Task<Payment> GetAsync(Guid id);

        Task<Payment> PostAsync(CreatePayment createPayment);

        Task<IEnumerable<Payment>> GetAsync(string cardNumber, string reference);
    }
}
