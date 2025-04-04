namespace PawaPayGateway.Domain.Models.Paystack
{
    public class PaystackRequest
    {
        public string amount { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
        public string callback_url { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
    }
}
