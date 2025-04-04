using PawaPayGateway.Domain.Responses;

namespace PawaPayGateway.Domain.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBankTransactionProvider : ITransactionProvider
    {
        Task<PaymentProvider> ReceiveFundsDetails(ReceiveFundsRequest request);

        Task<RecievePaymentResponseModel> ReceiveFundsLink(ReceiveFundsRequest request);

        Task<GatewayBaseResponseModel> SendFunds(string bankCode, string accountNumber, decimal amount, string reference, string accountName = null, string narration = null);

        //Gateway State Management Section....

        Task<ApiBaseResponse<TransactionStatusResponse>> QueryTransaction(string transactionReference, string collectionReference);

        Task<ApiBaseResponse<ValidateAccountNumberResponse>> GetAccountName(string accountNumber, string bankCode);
    }

    
}
