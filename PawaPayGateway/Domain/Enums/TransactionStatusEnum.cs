namespace PawaPayGateway.Domain.Enums
{
    public enum TransactionStatusEnum
    {
        SUCCESS = 1,
        CANCELLED,
        FAILED,
        PENDING,
        UNKNOWN,
        NOT_FOUND
    }
}