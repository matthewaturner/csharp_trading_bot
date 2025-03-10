using Bot.Analyzers;
using Bot.Helpers;
using Bot.Models.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.Results;

/// <summary>
/// Results of a backtest or a live run.
/// </summary>
public class RunResult
{
    public List<PortfolioSnapshot> PortfolioSnapshots { get; set; } = new();

    public List<double> Returns { get; private set; }

    public List<double> CumulativeReturns { get; private set; }

    public List<double> ExcessReturns { get; private set; }

    public List<double> HighWaterMark { get; private set; }

    public List<double> Drawdown { get; private set; }

    public List<double> DrawdownDuration { get; private set; }

    public double AnnualizedSharpeRatio = double.NaN;
    public double MaximumDrawdown = 0;
    public double MaximumDrawdownDuration = 0;

    /// <summary>
    /// Calculate all the values.
    /// </summary>
    public void CalculateResults(double annualRiskFreeRate, Interval interval)
    {
        IEnumerable<double> portfolioValue = PortfolioSnapshots.Select(s => s.PortfolioValue);
        Returns = [.. portfolioValue.Zip(portfolioValue.Skip(1), (prev, current) => (current - prev) / prev)];

        // this (annual risk free rate / intervals per year) seems like a simplification (no compounding) but it's fine for now
        ExcessReturns = [.. Returns.Select(r => r - (annualRiskFreeRate / interval.GetIntervalsPerYear()))];

        CumulativeReturns = [.. ExcessReturns.Aggregate(new List<double> { 0 }, (acc, r) => 
        { 
            acc.Add((1 + acc.Last()) * (1 + r) - 1); 
            return acc; 
        }).Skip(1)];

        HighWaterMark = [.. CumulativeReturns.Aggregate(new List<double> { 0 }, (acc, r) =>
        {
            acc.Add(Math.Max(acc.Last(), r));
            return acc;
        }).Skip(1)];

        Drawdown = [.. CumulativeReturns.Zip(HighWaterMark, (c, h) => (1 + c) / (1 + h) - 1)];

        DrawdownDuration = [.. Drawdown.Aggregate(new List<double> { 0 }, (acc, d) =>
        {
            acc.Add(d < 0 ? acc.Last() + 1 : 0);
            return acc;
        }).Skip(1)];

        MaximumDrawdown = Drawdown.Min();
        MaximumDrawdownDuration = DrawdownDuration.Max();
        AnnualizedSharpeRatio = CalculateAnnualizedSharpeRatio(interval);

        // add the initial zeroes to everything
        Returns.Insert(0, 0);
        ExcessReturns.Insert(0, 0);
        CumulativeReturns.Insert(0, 0);
        HighWaterMark.Insert(0, 0);
        Drawdown.Insert(0, 0);
        DrawdownDuration.Insert(0, 0);
    }

    /// <summary>
    /// Calculate the annualized sharpe ratio.
    /// </summary>
    private double CalculateAnnualizedSharpeRatio(Interval interval)
    {
        double periodsPerYear = interval.GetIntervalsPerYear();
        double mean = ExcessReturns.Average();
        double stddev = MathHelpers.StdDev(ExcessReturns);
        return (mean / stddev) * Math.Sqrt(periodsPerYear);
    }
}

/// <summary>
/// Record that holds a timestamp and a value.
/// </summary>
public record DatedValue(DateTime Timestamp, double Value);
