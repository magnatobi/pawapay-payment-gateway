namespace PawaPayGateway.Domain.Responses.Paystack
{
    public class Data
    {
        public int id { get; set; }
        public string domain { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
        public int amount { get; set; }
        public string message { get; set; } = string.Empty;
        public DateTime paidAt { get; set; }
        public DateTime createdAt { get; set; }
    }

    public class PaystackPaymentStatus
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public Data data { get; set; } = new Data();
    }
}
