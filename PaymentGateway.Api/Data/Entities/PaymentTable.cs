using System;

namespace PaymentGateway.Api.Data.Entities
{
    public class PaymentTable
    {
        public Guid Id { get; set; }

        public int RowIndex { get; set; }

        public DateTimeOffset DateCreated { get; set; }

        public string CreatedBy { get; set; }

        public decimal Amount { get; set; }

        public string CurrencyCode { get; set; }

        public string CardNumber { get; set; }

        public string CardName { get; set; }

        public byte ExpiryMonth { get; set; }

        public short ExpiryYear { get; set; }

        public string Reference { get; set; }

        public string BankCode { get; set; }
    }
}
