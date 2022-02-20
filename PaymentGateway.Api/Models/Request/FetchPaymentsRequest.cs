namespace PaymentGateway.Api.Models.Request
{
    public class FetchPaymentsRequest
    {
        public string CardNumber { get; set; }

        public string Reference { get; set; }
    }
}
