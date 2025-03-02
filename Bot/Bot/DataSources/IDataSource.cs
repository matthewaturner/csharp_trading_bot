using Bot.Models;
using System.Threading.Tasks;
using System;
using Bot.Events;

namespace Bot.DataSources;

public interface IDataSource
{
    /// <summary>
    /// Market data event handlers. Handlers are registered by the engine, but this class sends the events.
    /// </summary>
    public event EventHandler<MarketDataEvent> MarketDataReceivers;

    /// <summary>
    /// Stream bars to the engine.
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    Task StreamBars(
        string symbol,
        Interval interval,
        DateTime start,
        DateTime? end);

    /// <summary>
    /// Get the latest bar for a given symbol;
    /// </summary>
    public Bar GetLatestBar(string symbol);
}
