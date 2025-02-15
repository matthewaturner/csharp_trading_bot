
using System;
using System.Text.Json.Serialization;

namespace Bot.DataSources.Alpaca.Models;

public class Bar
{
    [JsonPropertyName("t")]
    public string T { get; set; }

    [JsonPropertyName("o")]
    public double O { get; set; }

    [JsonPropertyName("h")]
    public double H { get; set; }

    [JsonPropertyName("l")]
    public double L { get; set; }
    
    [JsonPropertyName("c")]
    public double C { get; set; }

    [JsonPropertyName("v")]
    public long V { get; set; }

    public Bot.Models.Bar ToBotModel(string symbol)
    {
        return new Bot.Models.Bar(DateTime.Parse(T), symbol, O, H, L, C, V);
    }
}
