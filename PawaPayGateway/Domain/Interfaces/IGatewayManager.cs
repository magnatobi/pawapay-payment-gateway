using PawaPayGateway.Domain.Entities;

namespace PawaPayGateway.Domain.Interfaces
{
    public interface IGatewayManager
    {
        IEnumerable<ITransactionProvider> GetAllGateways();

        IEnumerable<ITransactionProvider> GetGatewaysByType(GatewayType gatewayType, bool getAll = false);

        ITransactionProvider GetGatewayById(int gatewayId);

        bool AddTransactionProvider(ITransactionProvider gateway);

        bool RemoveTransactionProvider(int gatewayId);

        void ClearTransactionProviders();

    }
}
