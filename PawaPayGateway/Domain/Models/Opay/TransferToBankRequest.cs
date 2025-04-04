using System.Text.Json.Serialization;

namespace PawaPayGateway.Domain.Models.Opay
{
    public class TransferToBankRequest
    {
        [JsonPropertyOrder(1)]
        public string amount { get; set; }

        [JsonPropertyOrder(2)]
        public string country { get; set; }

        [JsonPropertyOrder(3)]
        public string currency { get; set; }

        [JsonPropertyOrder(4)]
        public string reason { get; set; }

        [JsonPropertyOrder(5)]
        public TransferReceiver receiver { get; set; }

        [JsonPropertyOrder(6)]
        public string reference { get; set; }
    }

    public class TransferReceiver
    {
        [JsonPropertyOrder(1)]
        public string bankAccountNumber { get; set; }

        [JsonPropertyOrder(2)]
        public string bankCode { get; set; }

        [JsonPropertyOrder(3)]
        public string name { get; set; }
    }
}