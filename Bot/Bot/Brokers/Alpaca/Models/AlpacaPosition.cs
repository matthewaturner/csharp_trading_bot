// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Broker;
using Newtonsoft.Json;

namespace Bot.Brokers.Alpaca.Models;

public class AlpacaPosition : IPosition
{
    // alpaca fields 

    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("qty")]
    public string Quantity { get; set; }

    [JsonProperty("side")]
    public PositionType Side { get; set; }

    // IPosition fields

    [JsonIgnore]
    public PositionType Type => Side;

    [JsonIgnore]
    double IPosition.Quantity => double.Parse(Quantity);
}
