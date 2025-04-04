namespace PawaPayGateway.Domain.Responses.Paystack
{
    public class History
    {
        public string type { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public int time { get; set; }
    }

    public class Log
    {
        public int time_spent { get; set; }
        public int attempts { get; set; }
        public string authentication { get; set; } = string.Empty;
        public int errors { get; set; }
        public bool success { get; set; }
        public bool mobile { get; set; }
        public IList<object> input { get; set; }
        public object channel { get; set; }
        public IList<History> history { get; set; }
    }

    public class Customer
    {
        public int id { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string customer_code { get; set; } = string.Empty;
        public object phone { get; set; }
        public object metadata { get; set; }
        public string risk_action { get; set; } = string.Empty;
    }

    public class Authorization
    {
        public string authorization_code { get; set; } = string.Empty;
        public string bin { get; set; } = string.Empty;
        public string last4 { get; set; } = string.Empty;
        public string exp_month { get; set; } = string.Empty;
        public string exp_year { get; set; } = string.Empty;
        public string card_type { get; set; } = string.Empty;
        public string bank { get; set; } = string.Empty;
        public string country_code { get; set; } = string.Empty;
        public string brand { get; set; } = string.Empty;
    }

    public class Plan
    {
    }

    public class PaystackChargeResponseData
    {
        public int id { get; set; }
        public string domain { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string reference { get; set; } = string.Empty;
        public int amount { get; set; }
        public object message { get; set; }
        public string gateway_response { get; set; } = string.Empty;
        public DateTime paid_at { get; set; }
        public DateTime created_at { get; set; }
        public string channel { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string ip_address { get; set; } = string.Empty;
        public string metadata { get; set; } = string.Empty;
        public Log log { get; set; }
        public object fees { get; set; }
        public Customer customer { get; set; }
        public Authorization authorization { get; set; }
        public Plan plan { get; set; }
    }

    public class PaystackChargeResponse
    {
        public string @event { get; set; } = string.Empty;
        public PaystackChargeResponseData? data { get; set; }
    }
}
