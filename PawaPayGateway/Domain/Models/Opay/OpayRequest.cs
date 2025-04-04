namespace PawaPayGateway.Domain.Models.Opay
{
    public class Receiver
    {
        public string name { get; set; }
        public int bankCode { get; set; }
        public int bankAccountNumber { get; set; }
    }

    public class OpayRequest
    {
        public string reference { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string country { get; set; }
        public Receiver receiver { get; set; }
        public string reason { get; set; }
    }
}