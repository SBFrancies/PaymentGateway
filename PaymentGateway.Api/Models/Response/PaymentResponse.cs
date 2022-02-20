using System;
using System.Collections.Generic;

namespace PaymentGateway.Api.Models.Response
{
    public class PaymentResponse
    {
        public PaymentDetails Payment { get; set; }

        public IEnumerable<PaymentEvent> Events { get; set; }

        public DateTimeOffset ResponseTime { get; set; }
    }
}
