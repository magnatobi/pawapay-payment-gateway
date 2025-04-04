using PawaPayGateway.Domain.Entities.Common;

namespace PawaPayGateway.Domain.Entities
{
    public class Gateway : EntityBase
    {
        public string Name { get; set; } = string.Empty;
        public int Priority { get; set; }
        public GatewayStatus Status { get; set; }
        public string CredentialKey { get; set; } = string.Empty;
        public string GatewayUrl { get; set; } = string.Empty;
        public GatewayType GatewayType { get; set; }
        public decimal Balance { get; set; }
    }

    public enum GatewayType : int
    {
        Paystack = 1,
        FlutterWave = 2,
        Squad = 3,
        Opay = 4,
        Remitta = 5
    }

    public enum GatewayStatus : int
    {
        Online = 1,
        Offline,
        Error,
        Disabled,
        Checking
    }
}
