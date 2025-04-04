namespace PawaPayGateway.Domain.Responses.Opay
{
    public class OpayResponse
    {
        public string reference { get; set; }
        public string orderNo { get; set; }
        public int amount { get; set; }
        public string currency { get; set; }
        public string fee { get; set; }
        public int status { get; set; }
        public string failureReason { get; set; }
        public int bankCode { get; set; }
        public int bankAccountNumber { get; set; }
    }
}