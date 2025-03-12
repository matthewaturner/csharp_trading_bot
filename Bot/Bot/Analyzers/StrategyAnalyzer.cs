// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Brokers;
using Bot.Engine;
using Bot.Events;
using Bot.Models.Engine;
using Bot.Models.Results;
using System;

namespace Bot.Analyzers;

public class StrategyAnalyzer(
    double annualRiskFreeRate = 0) 
    : IStrategyAnalyzer
{
    // private
    private ITradingEngine Engine;
    private double AnnualRiskFreeRate = annualRiskFreeRate;

    // public
    public RunResult RunResult { get; private set; } = new();

    /// <summary>
    /// Broker shorthand.
    /// </summary>
    private IBroker Broker => Engine.Broker;

    /// <summary>
    /// Interval shorthand.
    /// </summary>
    private Interval Interval => Engine.RunConfig.Interval;

    /// <summary>
    /// Handle initialize event.
    /// </summary>
    public void OnInitialize(object sender, EventArgs _)
    {
        Engine = sender as ITradingEngine;
    }

    /// <summary>
    /// Handle market data event.
    /// </summary>
    public void OnMarketData(object sender, MarketDataEvent e)
    {
        PortfolioSnapshot snapshot = Broker.GetPortfolio().GetSnapshot(e.Snapshot.Timestamp);
        RunResult.PortfolioSnapshots.Add(snapshot);
    }

    /// <summary>
    /// Calculate all the final values. At least in backtest mode, it would be pointless to calculate any of these
    /// before we are finished running.
    /// </summary>
    public void OnFinalize(object sender, EventArgs _)
    {
        RunResult.CalculateResults(AnnualRiskFreeRate, Interval);
    }
}
