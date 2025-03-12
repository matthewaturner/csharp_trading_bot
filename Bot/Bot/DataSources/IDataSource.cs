// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Events;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using System;
using System.Threading.Tasks;

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
    Task StreamBars(
        string[] symbols,
        Interval interval,
        DateTime start,
        DateTime? end);

    /// <summary>
    /// Get the latest bar for a given symbol;
    /// </summary>
    public Bar GetLatestBar(string symbol);
}
