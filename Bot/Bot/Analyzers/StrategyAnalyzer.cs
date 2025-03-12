// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Engine;
using Bot.Events;
using Bot.Models.Allocations;
using Bot.Models.Engine;
using Bot.Models.Results;
using System;

namespace Bot.Analyzers;

public class StrategyAnalyzer() : IStrategyAnalyzer
{
    private ITradingEngine Engine;

    public RunResult RunResult { get; private set; }

    /// <summary>
    /// Handle initialize event.
    /// </summary>
    public void OnInitialize(object sender, EventArgs _)
    {
        Engine = sender as ITradingEngine;
        RunResult = new RunResult(Engine.RunConfig.Universe);
    }

    /// <summary>
    /// Handle market data event.
    /// </summary>
    public void OnMarketData(object sender, MarketDataEvent e)
    {
        Allocation flatAllocations = Engine.MetaAllocation.FlattenAllocations();

        foreach (string symbol in e.Snapshot.Symbols)
        {
            RunResult.Timestamps.Add(e.Snapshot.Timestamp);
            RunResult.UnderlyingPrices[symbol].Add(e.Snapshot[symbol].AdjClose);
            RunResult.SymbolWeights[symbol].Add(flatAllocations[symbol]);

        }
    }

    /// <summary>
    /// Calculate all the final values. At least in backtest mode, it would be pointless to calculate any of these
    /// before we are finished running.
    /// </summary>
    public void OnFinalize(object sender, EventArgs _)
    {
        RunResult.CalculateResults(Engine.RunConfig.AnnualRiskFreeRate, Engine.RunConfig.Interval);
    }
}
