
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
        decimal IAccount.BuyingPower => decimal.Parse(BuyingPower);

        [JsonIgnore]
        decimal IAccount.Cash => decimal.Parse(Cash);

        [JsonIgnore]
        decimal IAccount.TotalValue => decimal.Parse(Equity);
    }
}
