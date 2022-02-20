using PaymentGateway.Api.Enums;
using PaymentGateway.Api.Models.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentGateway.Api.Interface
{
    public interface IEventStore
    {
        Task CreatePaymentEventAsync(Guid id, PaymentStatus key);

        Task<IEnumerable<PaymentEvent>> GetPaymentEventsAsync(Guid id);
    }
}
