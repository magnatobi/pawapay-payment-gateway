namespace PawaPayGateway.Domain.Responses.Opay
{
    internal class TransferToBankResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public TransferToBankData data { get; set; }
    }

    internal class TransferToBankData
    {
        public string reference { get; set; }
        public string orderNo { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string fee { get; set; }
        public string status { get; set; }
        public string failureReason { get; set; }
        public string bankCode { get; set; }
        public string bankAccountNumber { get; set; }
    }
}