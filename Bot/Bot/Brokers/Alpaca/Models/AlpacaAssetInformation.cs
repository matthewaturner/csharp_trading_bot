using Bot.Models.Broker;
using Newtonsoft.Json;

namespace Bot.Brokers.Alpaca.Models;

public class AlpacaAssetInformation : IAssetInformation
{
    // Alpaca fields

    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("class")]
    public string Class { get; set; }

    [JsonProperty("exchange")]
    public string Exchange { get; set; }

    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("tradable")]
    public bool Tradable { get; set; }

    [JsonProperty("marginable")]
    public bool Marginable { get; set; }

    [JsonProperty("shortable")]
    public bool Shortable { get; set; }

    [JsonProperty("easy_to_borrow")]
    public bool EasyToBorrow { get; set; }

    [JsonProperty("fractionable")]
    public bool Fractionable { get; set; }
}
