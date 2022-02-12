using PaymentGateway.Api.Enums;

namespace PaymentGateway.Api.Models.Request
{
    public class FetchPaymentsRequest
    {
        public string CardNumber { get; set; }

        public string Reference { get; set; }

        public PaymentStatus Status { get; set; }
    }
}
