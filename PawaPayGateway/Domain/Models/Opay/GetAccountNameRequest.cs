namespace PawaPayGateway.Domain.Models.Opay
{
    internal class GetAccountNameRequest
    {
        public string bankCode { get; set; }
        public string bankAccountNo { get; set; }
        public string countryCode { get; set; }
    }
}