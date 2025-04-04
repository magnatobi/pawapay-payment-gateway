namespace PawaPayGateway.Domain.Enums
{
    public enum EventTypeEnum
    {
        None = 0,

        //Bank Account
        BankAccountCreatedEvent = 1,
        BankAccountCreditedEvent = 2,
        BankAccountDebitedEvent = 3,
        BankAccountDeletedEvent = 4,

        //User Account
        UserAccountCreatedEvent = 5,
        UserAccountUpdatedEvent = 6,
        UserAccountDeletedEvent = 7,
    }
}
