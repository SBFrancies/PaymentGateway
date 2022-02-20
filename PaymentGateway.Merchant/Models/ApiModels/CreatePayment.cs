﻿namespace PaymentGateway.Merchant.Models.ApiModels
{
    public class CreatePayment
    {
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }

        public string CardName { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public string Cvv { get; set; }

        public string Reference { get; set; }

        public string CurrencyCode {get;set;}
    }
}
