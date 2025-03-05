using Bot.Brokers;
using Bot.Engine;
using Bot.Events;
using Bot.Helpers;
using Bot.Models.Engine;
using Bot.Models.Results;
using System;
using System.Linq;

namespace Bot.Analyzers;

public class StrategyAnalyzer(double annualRiskFreeRate = 0) : IStrategyAnalyzer
{
    // private
    private ITradingEngine Engine;
    private double AnnualRiskFreeRate = annualRiskFreeRate;

    // public
    public RunResult RunResults { get; private set; } = new();

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
        bool isFirstBar = RunResults.PortfolioValues.Count == 0;
        DateTime currentTime = e.Snapshot.Timestamp;

        double currentPortfolioValue = (double)Broker.GetAccount().TotalValue;
        double previousPortfolioValue = RunResults.PortfolioValues.LastOrDefault()?.Value ?? currentPortfolioValue;
        RunResults.PortfolioValues.Add(currentTime, currentPortfolioValue);

        if (!isFirstBar)
        {
            double dailyReturn = (currentPortfolioValue - previousPortfolioValue) / previousPortfolioValue;
            RunResults.DailyReturns.Add(currentTime, dailyReturn);

            double excessDailyReturn = dailyReturn - (AnnualRiskFreeRate / Interval.GetIntervalsPerYear());
            RunResults.ExcessDailyReturns.Add(currentTime, excessDailyReturn);

            double prevCumulativeReturn = RunResults.CumulativeReturns.LastOrDefault()?.Value ?? 0;
            double cumulativeReturn = (1 + prevCumulativeReturn) * (1 + dailyReturn) - 1;
            RunResults.CumulativeReturns.Add(currentTime, cumulativeReturn);

            double highWaterMark = RunResults.HighWaterMark.LastOrDefault()?.Value ?? 0;
            if (cumulativeReturn > highWaterMark) highWaterMark = cumulativeReturn;
            RunResults.HighWaterMark.Add(currentTime, highWaterMark);

            double drawdown = (1 + cumulativeReturn) / (1 + highWaterMark) - 1;
            RunResults.Drawdown.Add(currentTime, drawdown);

            double prevDrawdownDuration = RunResults.DrawdownDuration.LastOrDefault()?.Value ?? 0;
            double drawdownDuration = drawdown < 0 ? prevDrawdownDuration + 1 : 0;
            RunResults.DrawdownDuration.Add(currentTime, drawdownDuration);
        }
        else
        {
            RunResults.DailyReturns.Add(currentTime, 0);
            RunResults.ExcessDailyReturns.Add(currentTime, 0);
            RunResults.CumulativeReturns.Add(currentTime, 0);
            RunResults.HighWaterMark.Add(currentTime, 0);
            RunResults.Drawdown.Add(currentTime, 0);
            RunResults.DrawdownDuration.Add(currentTime, 0);
        }
    }

    /// <summary>
    /// Calculate all the final values. At least in backtest mode, it would be pointless to calculate any of these
    /// before we are finished running.
    /// </summary>
    public void OnFinalize(object sender, EventArgs _)
    {
        RunResults.AnnualizedSharpeRatio = CalculateAnnualizedSharpeRatio();
        RunResults.MaximumDrawdown = RunResults.Drawdown.Values().Min();
        RunResults.MaximumDrawdownDuration = RunResults.DrawdownDuration.Values().Max();
    }

    /// <summary>
    /// Calculate the annualized sharpe ratio.
    /// </summary>
    /// <returns></returns>
    public double CalculateAnnualizedSharpeRatio()
    {
        double periodsPerYear = Interval.GetIntervalsPerYear();
        double riskFreeRate = AnnualRiskFreeRate != 0 ? AnnualRiskFreeRate / periodsPerYear : 0;
        double mean = RunResults.ExcessDailyReturns.Values().Average();
        double stddev = MathHelpers.StdDev(RunResults.ExcessDailyReturns.Values());
        return (mean / stddev) * Math.Sqrt(periodsPerYear);
    }
}
