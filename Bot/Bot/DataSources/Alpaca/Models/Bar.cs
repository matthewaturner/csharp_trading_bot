
using System;
using System.Text.Json.Serialization;

namespace Bot.DataSources.Alpaca.Models;

public class Bar
{
    [JsonPropertyName("t")]
    public string T { get; set; }

    [JsonPropertyName("o")]
    public decimal O { get; set; }

    [JsonPropertyName("h")]
    public decimal H { get; set; }

    [JsonPropertyName("l")]
    public decimal L { get; set; }
    
    [JsonPropertyName("c")]
    public decimal C { get; set; }

    [JsonPropertyName("v")]
    public long V { get; set; }

    public Bot.Models.Bar ToBotModel(string symbol)
    {
        return new Bot.Models.Bar(DateTime.Parse(T), symbol, O, H, L, C, V, null);
    }
}
