namespace PawaPayGateway.Domain.Models.Opay
{
    internal class InitializeCheckoutRequest
    {
        public string reference { get; set; }
        public string mchShortName { get; set; }
        public string productName { get; set; }
        public string productDesc { get; set; }
        public string userPhone { get; set; }
        public string userRequestIp { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string[] payTypes { get; set; }
        public string[] payMethods { get; set; }
        public string callbackUrl { get; set; }
        public string returnUrl { get; set; }
        public string expireAt { get; set; }
    }
}