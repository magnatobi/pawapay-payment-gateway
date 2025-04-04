using System;
using System.Text.Json.Serialization;

namespace PawaPayGateway.Domain.Models.Opay
{
    public class Payload
    {
        public string country { get; set; }

        [JsonPropertyName("instrumentid")]
        public string instrumentId_ { get; set; }

        public string fee { get; set; }
        public string channel { get; set; }
        public string displayedFailure { get; set; }
        public string reference { get; set; }
        public DateTime updated_at { get; set; }
        public string currency { get; set; }
        public bool refunded { get; set; }

        [JsonPropertyName("instrument-id")]
        public string InstrumentId { get; set; }

        public DateTime timestamp { get; set; }
        public string amount { get; set; }
        public string sessionId { get; set; }
        public string instrumentType { get; set; }
        public string instrument_id { get; set; }
        public string transactionId { get; set; }
        public string token { get; set; }
        public string bussinessType { get; set; }
        public string payChannel { get; set; }
        public string status { get; set; }
    }

    public class CallbackRequest
    {
        public Payload payload { get; set; }
        public string sha512 { get; set; }
        public string type { get; set; }
    }
}