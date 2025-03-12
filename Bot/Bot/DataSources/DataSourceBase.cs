// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Events;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DataSources;

public abstract class DataSourceBase : IDataSource
{
    private MarketSnapshot CurrentSnapshot;

    public event EventHandler<MarketDataEvent> MarketDataReceivers;

    /// <summary>
    /// Stream bars to the engine.
    /// </summary>
    async Task IDataSource.StreamBars(
        string[] symbols,
        Interval interval,
        DateTime start,
        DateTime? end)
    {
        end = end.HasValue ? end.Value : DateTime.UtcNow;

        Dictionary<string, List<Bar>> barsDict = new();

        foreach (string symbol in symbols)
        {
            var bars = await GetHistoricalBarsAsync(symbol, interval, start, end.Value);
            barsDict.Add(symbol, bars);
        }

        var snapshots = barsDict.SelectMany(kv => kv.Value)
            .GroupBy(b => b.Timestamp)
            .OrderBy(g => g.Key)
            .Select(g => new MarketSnapshot(g.Key, g.ToArray()));

        foreach (MarketSnapshot snapshot in snapshots)
        {
            CurrentSnapshot = snapshot;
            MarketDataReceivers?.Invoke(this, new MarketDataEvent(snapshot));
        }
    }

    /// <summary>
    /// Get the latest bar for a given symbol.
    /// </summary>
    Bar IDataSource.GetLatestBar(string symbol)
    {
        return CurrentSnapshot[symbol];
    }

    /// <summary>
    /// Gets a list of bars for a symbol, interval, and period of time.
    /// </summary>
    public abstract Task<List<Bar>> GetHistoricalBarsAsync(
        string symbol,
        Interval interval,
        DateTime start,
        DateTime end);
}
