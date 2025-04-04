namespace PawaPayGateway.Domain.Responses.Opay
{
    internal class QueryBalanceResponse
    {
        public string code { get; set; }
        public string message { get; set; }
        public BalanceData data { get; set; }
    }

    internal class BalanceData
    {
        public CashBalance cashBalance { get; set; }
        public BonusBalance bonusBalance { get; set; }
        public OWealthBalance oWealthBalance { get; set; }
        public FreezeBalance freezeBalance { get; set; }
        public CashBackBalance cashBackBalance { get; set; }
        public CommissionBalance commissionBalance { get; set; }
        public Opos opos { get; set; }
    }

    internal class CashBalance
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    internal class BonusBalance
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    internal class OWealthBalance
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    internal class FreezeBalance
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    internal class CashBackBalance
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    internal class CommissionBalance
    {
        public string currency { get; set; }
        public string value { get; set; }
    }

    internal class Opos
    {
        public string currency { get; set; }
        public string value { get; set; }
    }
}