using System;

namespace PaymentGateway.Api.Models.BankApi
{
    public class CardPaymentResponse
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public DateTime? DateProcessed { get; set; }
    }
}
