using System;

namespace PaymentGateway.Api.Models.Response
{
    public class PaymentDetails
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }

        public string CardName { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public string CurrencyCode { get; set; }

        public string Reference { get; set; }

        public string BankCode { get; set; }
    }
}
