
using Bot.Models.Interfaces;
using Newtonsoft.Json;

namespace Bot.Brokers.Alpaca.Models
{
    public class AlpacaAccount : IAccount
    {
        // Alpaca fields

        [JsonProperty("account_number")]
        public string AccountId { get; set; }

        [JsonProperty("buying_power")]
        public string BuyingPower { get; set; }

        [JsonProperty("cash")]
        public string Cash { get; set; }

        [JsonProperty("equity")]
        public string Equity { get; set; }

        // IAccount interface

        [JsonIgnore]
        double IAccount.BuyingPower => double.Parse(BuyingPower);

        [JsonIgnore]
        double IAccount.Cash => double.Parse(Cash);

        [JsonIgnore]
        double IAccount.TotalValue => double.Parse(Equity);
    }
}
