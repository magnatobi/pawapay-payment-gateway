using PawaPayGateway.Domain.Entities;
using System.Text.Json.Serialization;

namespace PawaPayGateway.Domain.Responses
{
    public class PaymentDetail
    {
        public decimal RequestedAmount { get; set; }
        public decimal TotalCharged { get; set; }
        public dynamic Fee { get; set; }
        public string CollectionReference { get; set; } = string.Empty; 
        public string TransactionReference { get; set; } = string.Empty ;
        public string ReceiptEmail { get; set; } = string.Empty;
        public PaymentProvider[] PaymentProviders { get; set; }
    }

    public class PaymentProvider
    {
        public int GatewayId { get; set; }
        public string Name { get; set; } = string.Empty;
        public GatewayType GatewayType { get; set; }
        public string PaymentLink { get; set; } = string.Empty;
        public string Key1 { get; set; } = string.Empty;
        public string Key2 { get; set; } = string.Empty;
        public string Key3 { get; set; } = string.Empty;
    }

    public class PaymentResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaymentResponseData? Data { get; set; }
    }

    public class PaymentResponseData
    {
        public string authorization_url { get; set; } = string.Empty;
        public string access_code { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
    }
}
