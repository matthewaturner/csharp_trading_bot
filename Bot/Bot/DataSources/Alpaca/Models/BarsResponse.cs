using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Bot.DataSources.Alpaca.Models;

public class BarsResponse
{
    [JsonPropertyName("bars")]
    public List<Bar> Bars { get; set; }
}