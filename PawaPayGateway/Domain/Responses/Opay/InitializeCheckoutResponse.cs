namespace PawaPayGateway.Domain.Responses.Opay
{
    internal class InitializeCheckoutResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public Data data { get; set; }
    }

    internal class Data
    {
        public string reference { get; set; }
        public string orderNo { get; set; }
        public string cashierUrl { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string status { get; set; }
    }
}