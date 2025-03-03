using Bot.Brokers;
using Bot.Engine;
using Bot.Events;
using Bot.Helpers;
using Bot.Models.Results;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Analyzers;

public class StrategyAnalyzer(double annualRiskFreeRate = 0) : IAnalyzer
{
    private ITradingEngine Engine;
    private IBroker Broker => Engine.Broker;

    private double AnnualRiskFreeRate = annualRiskFreeRate;

    // the object we are defining
    public RunResult RunResults { get; private set; } = new();

    public void Initialize(ITradingEngine engine)
    {
        Engine = engine;
    }

    /// <summary>
    /// Update the RunResults every time we receive market data.
    /// </summary>
    public void OnMarketData(object sender, MarketDataEvent e)
    {
        DateTime currentTime = e.Bar.Timestamp;

        double currentPortfolioValue = (double)Broker.GetAccount().TotalValue;
        double previousPortfolioValue = RunResults.PortfolioValues.LastOrDefault()?.Value ?? currentPortfolioValue;
        RunResults.PortfolioValues.Add(currentTime, currentPortfolioValue);

        double dailyReturn = (currentPortfolioValue - previousPortfolioValue) / previousPortfolioValue;
        RunResults.DailyReturns.Add(currentTime, dailyReturn);

        double excessDailyReturn = dailyReturn - (AnnualRiskFreeRate / Engine.Interval.GetIntervalsPerYear());
        RunResults.ExcessDailyReturns.Add(currentTime, excessDailyReturn);

        double prevCumulativeReturn = RunResults.CumulativeReturns.LastOrDefault()?.Value ?? 0;
        double cumulativeReturn = (1 + prevCumulativeReturn) * (1 + dailyReturn) - 1;
        RunResults.CumulativeReturns.Add(currentTime, cumulativeReturn);
    }

    /// <summary>
    /// Calculate all the final values. At least in backtest mode, it would be pointless to calculate any of these
    /// before we are finished running.
    /// </summary>
    public void OnFinalize(object sender, FinalizeEvent e)
    {
        RunResults.AnnualizedSharpeRatio = CalculateAnnualizedSharpeRatio();
    }

    #region Final Calculations =======================================================================================

    public double CalculateAnnualizedSharpeRatio()
    {
        double periodsPerYear = Engine.Interval.GetIntervalsPerYear();
        double riskFreeRate = AnnualRiskFreeRate != 0 ? AnnualRiskFreeRate / periodsPerYear : 0;

        IEnumerable<double> excessReturns = RunResults.DailyReturns.Values().Select(v => v - riskFreeRate);

        double mean = excessReturns.Average();
        double stddev = MathHelpers.StandardDeviation(excessReturns);
        return (mean / stddev) * Math.Sqrt(periodsPerYear);
    }

    #endregion

}
