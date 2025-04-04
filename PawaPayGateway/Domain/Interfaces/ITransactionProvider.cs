using PawaPayGateway.Domain.Entities;
using PawaPayGateway.Domain.Responses;

namespace PawaPayGateway.Domain.Interfaces
{
    public interface ITransactionProvider
    {
        int GatewayId { get; }
        string Name { get; set; } 
        string CredentialKey { get; }
        string Url { get; }
        int Priority { get; set; }
        GatewayStatus Status { get; set; }
        GatewayType GatewayType { get; }

        void ProcessError(string httpStatusCode, string message);

        Task<GatewayBaseResponseModel> CheckAvailability();

        Task<GatewayBalanceResponse> QueryGatewayBalance();
    }
}
