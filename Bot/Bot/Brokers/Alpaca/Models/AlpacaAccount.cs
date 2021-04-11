
using Bot.Models.Interfaces;
using Newtonsoft.Json;

namespace Bot.Brokers.Alpaca.Models
{
    public class AlpacaAccount : IAccount
    {
        [JsonProperty("account_number")]
        public string AccountId { get; set; }

        [JsonProperty("buying_power")]
        public string BuyingPower { get; set; }

        [JsonProperty("cash_balance")]
        public string CashBalance { get; set; }

        [JsonProperty("equity")]
        public string Equity { get; set; }

        double IAccount.BuyingPower
        {
            get { return double.Parse(BuyingPower); }
        }

        double IAccount.CashBalance
        {
            get { return double.Parse(CashBalance); }
        }

        double IAccount.TotalValue
        {
            get { return double.Parse(Equity); }
        }
    }
}
