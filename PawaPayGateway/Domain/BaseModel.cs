using System.ComponentModel;

namespace PawaPayGateway.Domain
{
    public class MailRequest
    {
        public string To { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty ;
        public string BodyText { get; set; } = string.Empty;
        public string BodyHtml { get; set; } = string.Empty;
    }

    public class ReceiveFundsRequest
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserRequestIPAddress { get; set; } = string.Empty;
        public string CallbackUrl { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public bool DoGenerateLinks { get; set; }
    }

    public class TransferModelRequest
    {
        public string account_bank { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public int amount { get; set; }
        public string narration { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
        public string callback_url { get; set; } = string.Empty;
        public string debit_currency { get; set; } = string.Empty;
    }

    public class CreateRecipientRequest
    {
        public string type { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string account_number { get; set; } = string.Empty;
        public string bank_code { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
    }

    public class Details
    {
        public string account_number { get; set; } = string.Empty;
        public object account_name { get; set; }
        public string bank_code { get; set; } = string.Empty;
        public string bank_name { get; set; } = string.Empty;
    }

    public class Recipient
    {
        public string domain { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public Details details { get; set; }
        public string description { get; set; } = string.Empty;
        public string metadata { get; set; } = string.Empty;
        public string recipient_code { get; set; } = string.Empty;
        public bool active { get; set; }
        public object email { get; set; }
        public int id { get; set; }
        public int integration { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class Data3
    {
        public int integration { get; set; }
        public Recipient recipient { get; set; }
        public string domain { get; set; } = string.Empty; 
        public int? amount { get; set; }
        public int? fees { get; set; }
        public string currency { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;    
        public string source { get; set; } = string.Empty;
        public object source_details { get; set; }
        public string reason { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public object failures { get; set; }
        public string transfer_code { get; set; } = string.Empty;
        public object titan_code { get; set; }
        public object transferred_at { get; set; }
        public ulong id { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class Data4
    {
        public string account_number { get; set; } = string.Empty;
        public string account_name { get; set; } = string.Empty;
        public int bank_id { get; set; }
    }

    public class AccountResolveResponse
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public Data4 data { get; set; }
    }

    public class TransferStatus
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public Data3 data { get; set; }
    }

    public class Details2
    {
        public object authorization_code { get; set; } 
        public string account_number { get; set; } = string.Empty;  
        public object account_name { get; set; }
        public string bank_code { get; set; } = string.Empty;
        public string bank_name { get; set; } = string.Empty;
    }

    public class CreateRecipientResponseData
    {
        public bool active { get; set; }
        public DateTime createdAt { get; set; }
        public string currency { get; set; } = string.Empty;
        public string domain { get; set; } = string.Empty;
        public int id { get; set; }
        public int integration { get; set; }
        public string name { get; set; } = string.Empty;
        public string recipient_code { get; set; } = string.Empty;
        public string type { get; set; } = string.Empty;
        public DateTime updatedAt { get; set; }
        public bool is_deleted { get; set; }
        public Details2 details { get; set; }
    }

    public class CreateRecipientResponse
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty; 
        public CreateRecipientResponseData data { get; set; }
    }

    public class InitiateTransferRequest
    {
        public string reference { get; set; } = string.Empty;
        public string source { get; set; } = string.Empty;
        public string amount { get; set; } = string.Empty;
        public string recipient { get; set; } = string.Empty;
        public string reason { get; set; } = string.Empty;
    }

    public class InitiateTransferResponseData
    {
        public string reference { get; set; } = string.Empty;
        public int integration { get; set; }
        public string domain { get; set; } = string.Empty;                          
        public int amount { get; set; }
        public string currency { get; set; } = string.Empty;
        public string source { get; set; } = string.Empty;
        public string reason { get; set; } = string.Empty;
        public int recipient { get; set; }
        public string status { get; set; } = string.Empty;
        public string transfer_code { get; set; } = string.Empty;
        public int id { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class InitiateTransferResponse
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public InitiateTransferResponseData data { get; set; }
    }


    //Bad response from https://api.paystack.co/transfer:
    //{"status":false,
    //"message":"Please provide a unique reference. Reference already exists on a transfer",
    //"meta":{"nextStep":"Create a new reference for the new transfer"},
    //"type":"validation_error",
    //"code":"duplicate_transfer_reference"}
    public class BankTransferBadResponse
    {
        public bool status { get; set; }
        public string message { get; set; } = string.Empty;
        public BankTransferBadResponseMeta? meta { get; set; }
        public string type { get; set; } = string.Empty;
        public string code { get; set; } = string.Empty;
    }

    public class BankTransferBadResponseMeta
    {
        public string nextStep { get; set; } = "Create a new reference for the new transfer";
    }

    public enum BankTransferBadResponseTypeEnum
    {
        //public string nextStep { get; set; } = "Create a new reference for the new transfer";
        [Description("validation_error")]
        ValidationError = 1,
    }
}
