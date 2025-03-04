using Bot.Events;
using Bot.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.DataSources;

public abstract class DataSourceBase : IDataSource
{
    private ILogger Logger => GlobalConfig.GlobalLogger;

    private Bar CurrentBar;

    public event EventHandler<MarketDataEvent> MarketDataReceivers;

    /// <summary>
    /// Stream bars to the engine.
    /// </summary>
    async Task IDataSource.StreamBars(
        string symbol,
        Interval interval,
        DateTime start,
        DateTime? end)
    {
        end = end.HasValue ? end.Value : DateTime.UtcNow;

        var bars = await GetHistoricalBarsAsync(symbol, interval, start, end.Value);

        foreach (Bar b in bars)
        {
            CurrentBar = b;
            MarketDataReceivers?.Invoke(this, new MarketDataEvent(b));
        }
    }

    /// <summary>
    /// Get the latest bar for a given symbol.
    /// </summary>
    Bar IDataSource.GetLatestBar(string symbol)
    {
        return CurrentBar;
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
