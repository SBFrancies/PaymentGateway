using System;
using System.Collections.Generic;

namespace PaymentGateway.Merchant.Models.ApiModels
{
    public class PaymentResponse
    {
        public Payment Payment { get; set; }

        public IEnumerable<PaymentEvent> Events { get; set; }

        public DateTimeOffset ResponseTime { get; set; }
    }
}
