// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Engine;
using Bot.Models.Allocations;
using Bot.Models.MarketData;
using Microsoft.Extensions.Logging;
using System;

namespace Bot.Strategies;

public abstract class StrategyBase : IStrategy
{
    /// <summary>
    /// Logger for convenience.
    /// </summary>
    private ILogger Logger;

    /// <summary>
    /// Unique identifier for the strategy.
    /// </summary>
    public string Id { get; private set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Handle initialize event.
    /// </summary>
    public void OnInitialize(object sender, EventArgs _)
    {
        var engine = sender as ITradingEngine;
        this.Logger = engine.CreateLogger(this.GetType().Name);
    }

    /// <summary>
    /// Handler market data event.
    /// </summary>
    public Allocation OnMarketDataBase(MarketSnapshot snapshot)
    {
        Logger.LogDebug($"Received snapshot: {snapshot}");
        return OnMarketData(snapshot);
    }

    /// <summary>
    /// Implemented by strategy.
    /// </summary>
    public abstract Allocation OnMarketData(MarketSnapshot bar);
}
