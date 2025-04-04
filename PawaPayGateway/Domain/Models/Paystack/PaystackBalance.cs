using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PawaPayGateway.Domain.Models.Paystack
{
    public class PaystackBalance
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public List<Data> data { get; set; } = new List<Data>();
    }

    public class Data
    {
        public string currency { get; set; } = string.Empty;
        public int balance { get; set; }
    }
}
