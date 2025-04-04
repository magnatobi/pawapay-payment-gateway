namespace PawaPayGateway.Domain.Models.Flutterwave
{
    public class FlutterWaveRequest
    {
        public string account_bank { get; set; }
        public string account_number { get; set; }
        public int amount { get; set; }
        public string narration { get; set; }
        public string currency { get; set; }
        public string reference { get; set; }
        public string callback_url { get; set; }
        public string debit_currency { get; set; }
    }

    public class FlutterAirtimeDisbursementRequest
    {
        public string country { get; set; } = "NG";
        public string customer { get; set; }
        public int amount { get; set; }
        public string reccurrence { get; set; } = "Once";
        public string type { get; set; } = "Airtime";
        public string reference { get; set; }
    }

    public class FluterwaveReceivePaymentRequest
    {
        public string tx_ref { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string redirect_url { get; set; }
        public string payment_options { get; set; }
        public FlutterCustomer customer { get; set; }
    }

    public class FlutterCustomer
    {
        public string email { get; set; }
    }

    public class FlutterVerifyBankRequest
    {
        public string account_number { get; set; }
        public string account_bank { get; set; }
    }
}