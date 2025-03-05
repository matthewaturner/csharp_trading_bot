
using Bot.Models.Interfaces;
using Newtonsoft.Json;

namespace Bot.Brokers.Alpaca.Models;

public class AlpacaAccount : IPortfolio
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
    double IPortfolio.BuyingPower => double.Parse(BuyingPower);

    [JsonIgnore]
    double IPortfolio.Cash => double.Parse(Cash);

    [JsonIgnore]
    double IPortfolio.TotalValue => double.Parse(Equity);
}
