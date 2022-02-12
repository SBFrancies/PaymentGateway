using PaymentGateway.Api.Models.Request;
using PaymentGateway.Merchant.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Merchant.Interface
{
    public interface IGatewayApi
    {
        Task GetAsync(Guid id);

        Task<Payment> PostAsync(CreatePayment createPayment);

        Task<IEnumerable<Payment>> GetAsync(string cardNumber, string reference);
    }
}
