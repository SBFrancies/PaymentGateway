using System;

namespace PaymentGateway.Merchant.Models.ApiModels
{
    public class PaymentEvent
    {
        public string Status { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
    }
}
