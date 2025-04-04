using PawaPayGateway.Domain.Entities;
using PawaPayGateway.Domain.Enums;
using PawaPayGateway.Domain.Interfaces;
using System.Collections.Concurrent;

namespace PawaPayGateway.Infrastructure.Implementations
{
    /// <summary>
    ///
    /// </summary>
    public class GatewayManager : IGatewayManager
    {
        private static readonly ConcurrentDictionary<int, ITransactionProvider> TransactionProviders =
            new ConcurrentDictionary<int, ITransactionProvider>();

        private class GatewayTypeComparer : IEqualityComparer<ITransactionProvider>
        {
            public bool Equals(ITransactionProvider p1, ITransactionProvider p2)
            {
                if (p1 == null && p2 == null)
                    return true;
                if (p1 == null || p2 == null)
                    return false;
                return p1.GatewayType == p2.GatewayType;
            }

            public int GetHashCode(ITransactionProvider p)
            {
                return p.GatewayType.GetHashCode();
            }
        }

        private static readonly GatewayTypeComparer gatewayTypeComparer = new GatewayTypeComparer();

        public bool AddTransactionProvider(ITransactionProvider gateway)
        {
            //remove then add
            RemoveTransactionProvider(gateway.GatewayId);
            var res = TransactionProviders.TryAdd(gateway.GatewayId, gateway);
            return res;
        }

        public bool RemoveTransactionProvider(int gatewayId)
        {
            return TransactionProviders.TryRemove(gatewayId, out var _);
        }

        public void ClearTransactionProviders()
        {
            TransactionProviders.Clear();
        }

        public IEnumerable<ITransactionProvider> GetAllGateways()
        {
            return TransactionProviders.Values;
        }

        /// <summary>
        /// Get all gateways by provider type. Return only active gateways by default.
        /// </summary>
        /// <param name="gatewayType"></param>
        /// <param name="getAll">Set to true to ignore Status and return all</param>
        /// <returns></returns>
        public IEnumerable<ITransactionProvider> GetGatewaysByType(GatewayType gatewayType, bool getAll = false)
        {
            return TransactionProviders.Values.Where(p =>
                p.GatewayType == gatewayType
                && (getAll || p.Status == GatewayStatus.Online || p.Status == GatewayStatus.Checking));
        }

        /// <summary>
        /// Get a gateway from the cache.
        /// </summary>
        /// <param name="gatewayId"></param>
        /// <returns>An instance of a ITransactionProvider or Null if the gateway does not exist</returns>
        public ITransactionProvider GetGatewayById(int gatewayId)
        {
            return TransactionProviders.TryGetValue(gatewayId, out var gateway) ? gateway : null;
        }

        #region Private Methods

        /// <summary>
        /// Get a gateway for a specific use case
        /// </summary>
        /// <param name="gatewayUseCase"></param>
        /// <param name="amount"></param>
        /// <param name="exclusions"></param>
        /// <returns></returns>
        private ITransactionProvider GetProvider(GatewayUseCaseEnum gatewayUseCase, decimal amount, IEnumerable<int> exclusions = null)
        {
            var rand = new Random();
            var gw = TransactionProviders.Values.OrderBy(p => gatewayUseCase.PriorityField(p) + rand.NextDouble())
                .FirstOrDefault(p =>
                    gatewayUseCase.ClassType().IsInstanceOfType(p)
                    && gatewayUseCase.PriorityField(p) > 0
                    && p.Status == GatewayStatus.Online
                    && (exclusions == null || !exclusions.Contains(p.GatewayId)));

            if (gw == null)
            {
                if (exclusions == null)
                    throw new InvalidOperationException($"No Provider for {nameof(gatewayUseCase)}");

                //retry with no exclusions
                return GetProvider(gatewayUseCase, amount);
            }

            return gw;
        }

        #endregion Private Methods
    }
}