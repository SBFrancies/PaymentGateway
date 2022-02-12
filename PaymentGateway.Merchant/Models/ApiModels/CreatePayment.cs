namespace PaymentGateway.Api.Models.Request
{
    public class CreatePayment
    { 
        public string CardNumber { get; set; }

        public string CardName { get; set; }

        public int ExpiryMonth { get; set; }

        public int ExpiryYear { get; set; }

        public int Cvv { get; set; }

        public string Reference { get; set; }
    }
}
