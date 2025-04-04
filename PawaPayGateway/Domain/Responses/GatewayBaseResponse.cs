using PawaPayGateway.Domain.Entities;

namespace PawaPayGateway.Domain.Responses
{
    public class GatewayBaseResponseModel
    {
        public GatewayResponseEnum Status { get; set; }
        public string Message { get; set; } = string.Empty; 
        public dynamic Data { get; set; }
    }

    public class GatewayBalanceResponse : GatewayBaseResponseModel
    {
        public decimal Balance { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
    }

    public enum GatewayResponseEnum
    {
        Success = 1,
        Failed,
        Pending
    }

    public class RecievePaymentResponseModel
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public string link { get; set; } = string.Empty;
        public GatewayType platformType { get; set; }
    }
}
