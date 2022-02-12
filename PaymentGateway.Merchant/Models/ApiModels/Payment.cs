namespace PaymentGateway.Merchant.Models
{
    public class Payment
    {
        public string CardNumber { get; set; }

        public string CardName { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public string Reference { get; set; }
    }
}
