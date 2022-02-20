using System;
using System.Collections.Generic;

namespace PaymentGateway.Merchant.Models.ApiModels
{
    public class PaymentsResponse
    {
        public IEnumerable<Payment> Payments { get; set; }

        public DateTimeOffset ResponseTime { get; set; }
    }
}
