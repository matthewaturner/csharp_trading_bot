using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.Results;

/// <summary>
/// Results of a backtest or a live run.
/// </summary>
public class RunResult
{
    public List<DatedValue> PortfolioValues = new();
    public List<DatedValue> DailyReturns = new();
    public List<DatedValue> ExcessDailyReturns = new();
    public List<DatedValue> CumulativeReturns = new();

    public List<DatedValue> HighWaterMark = new();
    public List<DatedValue> Drawdown = new();
    public List<DatedValue> DrawdownDuration = new();

    public double AnnualizedSharpeRatio = double.NaN;
    public double MaximumDrawdown = 0;
    public double MaximumDrawdownDuration = 0;
}

/// <summary>
/// Record that holds a timestamp and a value.
/// </summary>
public record DatedValue(DateTime Timestamp, double Value);

/// <summary>
/// Extensions for simplifying usage.
/// </summary>
public static class ResultExtensions
{
    public static void Add(this List<DatedValue> list, DateTime timestamp, double value)
    {
        list.Add(new DatedValue(timestamp, value));
    }

    public static IEnumerable<double> Values(this List<DatedValue> list)
    {
        return list.Select(v => (double)v.Value);
    }
}

