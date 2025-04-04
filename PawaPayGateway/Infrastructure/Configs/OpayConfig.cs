namespace PawaPayGateway.Infrastructure.Configs
{
    public class OpayConfig
    {
        public string MerchantId { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;   
        public string Url { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;       
        public string[] AllowedIps { get; set; } = new string[0];   
    }
}
