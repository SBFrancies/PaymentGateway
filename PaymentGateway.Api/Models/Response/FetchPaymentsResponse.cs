using System;
using System.Collections.Generic;

namespace PaymentGateway.Api.Models.Response
{
    public class FetchPaymentsResponse
    {
        public IEnumerable<PaymentDetails> Payments { get; set; }

        public DateTimeOffset ResponseTime { get; set; }
    }
}
