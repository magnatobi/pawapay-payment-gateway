using PawaPayGateway.Domain.Enums;
using System.Transactions;

namespace PawaPayGateway.Domain.Responses
{
    public class TransactionStatusResponse
    {
        public TransactionStatusEnum TransactionStatus { get; set; }
        public string status { get; set; } = string.Empty;      
        public decimal? amount { get; set; }
        public decimal? settled_amount { get; set; }
        public string TransactionInfo { get; set; } = string.Empty;
        public string Narration { get; set; } = string.Empty;
        public string mReference { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string statusDescription { get; set; } = string.Empty;
    }
}
