using System.Text.Json.Serialization;

namespace PawaPayGateway.Domain.Models.Opay
{
    internal class QueryTransactionRequest
    {
        [JsonPropertyOrder(2)]
        public string reference { get; set; }

        [JsonPropertyOrder(1)]
        public string orderNo { get; set; }
    }
}