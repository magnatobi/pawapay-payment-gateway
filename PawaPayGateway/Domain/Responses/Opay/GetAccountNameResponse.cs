namespace PawaPayGateway.Domain.Responses.Opay
{
    internal class GetAccountNameResponse
    {
        public string code { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty; 
        public AccountDetails? data { get; set; }
    }

    internal class AccountDetails
    {
        public string accountNo { get; set; } = string.Empty;   
        public string accountName { get; set; } = string.Empty;
    }
}