namespace PawaPayGateway.Domain.Responses
{
    public class PrimeTransferToAccountRequest
    {
        public decimal amount { get; set; }
        public string customer_reference { get; set; } = string.Empty;
    }

    public class TransferToAccountResponse
    {
    }

    public class PrimeValidateAccountNumberResponse
    {
        public string target_accountNumber { get; set; } = string.Empty;
        public string target_accountName { get; set; } = string.Empty;
        public string target_bankCode { get; set; } = string.Empty;
        public string target_bankName { get; set; } = string.Empty;     
    }

    public class ValidateAccountNumberResponse
    {
        public object AccountName { get; set; }
        public object AccountNumber { get; set; }
    }
}
