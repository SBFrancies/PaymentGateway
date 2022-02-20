using PaymentGateway.Api.Models.Request;
using System;

namespace PaymentGateway.Api.Models.Response
{
    public class CreatePaymentResponse
    {
        public Guid PaymentId { get; set; }

        public DateTimeOffset DateProcessed { get; set; }

        public bool Success { get; set; }

        public string Message { get; set; }

        public CreatePaymentRequest Request { get; set; }
    }
}
