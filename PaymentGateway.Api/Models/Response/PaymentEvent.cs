using PaymentGateway.Api.Enums;
using System;

namespace PaymentGateway.Api.Models.Response
{
    public class PaymentEvent
    {
        public Guid Id { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
