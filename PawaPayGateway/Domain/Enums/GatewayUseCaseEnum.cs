using PawaPayGateway.Domain.Interfaces;

namespace PawaPayGateway.Domain.Enums
{
    public enum GatewayUseCaseEnum
    {
        Payment,
        SmsSend,
        HLRLookup
    }

    public static class Extensions
    {
        public static Type ClassType(this GatewayUseCaseEnum gatewayUseCase)
        {
            return gatewayUseCase switch
            {
                GatewayUseCaseEnum.Payment => typeof(IBankTransactionProvider),
                _ => throw new ArgumentOutOfRangeException(nameof(gatewayUseCase), gatewayUseCase, "Please implement me")
            };
        }

        public static int PriorityField(this GatewayUseCaseEnum gatewayUseCase, ITransactionProvider p)
        {
            return gatewayUseCase switch
            {
                GatewayUseCaseEnum.Payment => p.Priority,
                //GatewayUseCaseEnum.Payment => p.HlrPriority,
                //GatewayUseCaseEnum.Payment => p.SmsSendPriority,
                _ => throw new ArgumentOutOfRangeException(nameof(gatewayUseCase), gatewayUseCase, "Please implement me")
            };
        }
    }
}