namespace PawaPayGateway.Domain.Responses.Opay
{
    internal class QueryTransactionResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public TransactionData data { get; set; }
    }

    internal class TransactionData
    {
        public string reference { get; set; }
        public string orderNo { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string fee { get; set; }
        public string status { get; set; }
        public string failureReason { get; set; }
        public string bankAccountNumber { get; set; }
    }
}