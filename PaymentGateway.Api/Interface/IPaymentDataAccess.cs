using PaymentGateway.Api.Data.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Interface
{
    public interface IPaymentDataAccess
    {
        Task<PaymentTable> SaveNewPaymentAsync(PaymentTable payment);

        Task<PaymentTable> UpdatePaymentAsync(Guid paymentId, string bankCode);

        Task<IEnumerable<PaymentTable>> GetPaymentsAsync(string cardNumber, string reference);

        Task<PaymentTable> GetPaymentAsync(Guid id);
    }
}
