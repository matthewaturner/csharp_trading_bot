using Bot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.MarketData;

/// <summary>
/// Snapshot object containing multiple synchronized bars.
/// </summary>
public class MarketSnapshot
{
    public DateTime Timestamp { get; }

    protected Dictionary<string, Bar> Bars { get; } // Keyed by symbol

    public MarketSnapshot(DateTime timestamp, params Bar[] bars)
    {
        Timestamp = timestamp;
        Bars = bars.ToDictionary(b => b.Symbol, b => b);
    }

    public Bar this[string symbol] => Bars.ContainsKey(symbol) ? 
        Bars[symbol] : throw new KeyNotFoundException($"Symbol {symbol} not found in market snapshot.");

    public override string ToString()
    {
        return $"MarketSnapshot: {Timestamp.StdToString()} - {Bars.Count} bars";
    }
}
