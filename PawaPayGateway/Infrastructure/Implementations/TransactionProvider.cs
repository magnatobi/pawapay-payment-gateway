using PawaPayGateway.Domain.Entities;
using PawaPayGateway.Domain.Interfaces;
using PawaPayGateway.Domain.Responses;

namespace PawaPayGateway.Infrastructure.Implementations
{
    /// <summary>
    /// Convenience class to that implements the common fields
    /// </summary>
    public abstract class TransactionProvider : ITransactionProvider
    {
        public int GatewayId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Priority { get; set; }
        public string CredentialKey { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public GatewayStatus Status { get; set; }
        public abstract GatewayType GatewayType { get; }

        #region Methods

        public abstract void ProcessError(string httpStatusCode, string message);

        public abstract Task<GatewayBaseResponseModel> CheckAvailability();

        public abstract Task<GatewayBalanceResponse> QueryGatewayBalance();

        protected GatewayBalanceResponse QueryGatwewayAvailablity(GatewayBalanceResponse balance)
        {
            if (balance != null && balance.Balance > 0)
            {
                return new GatewayBalanceResponse { Status = GatewayResponseEnum.Success, Message = "Gateway is available" };
            }

            return new GatewayBalanceResponse { Status = GatewayResponseEnum.Failed, Message = "Gateway is unavailable" };
        }

        #endregion Methods
    }
}