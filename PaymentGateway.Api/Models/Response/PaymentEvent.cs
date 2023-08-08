using Newtonsoft.Json;
using PaymentGateway.Api.Enums;
using System;
using System.Text.Json.Serialization;

namespace PaymentGateway.Api.Models.Response
{
    public class PaymentEvent
    {
        [JsonPropertyName("id")]
        [JsonProperty("id")]
        public string Id { get; } = Guid.NewGuid().ToString();

        [JsonPropertyName("paymentId")]
        [JsonProperty("paymentId")]
        public Guid PaymentId { get; set; }

        [JsonPropertyName("status")]
        [JsonProperty("status")]
        public PaymentStatus Status { get; set; }

        [JsonPropertyName("timeStamp")]
        [JsonProperty("timeStamp")]
        public DateTimeOffset TimeStamp { get; set; }
    }
}
