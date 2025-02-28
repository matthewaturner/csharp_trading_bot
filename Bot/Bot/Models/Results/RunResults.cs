using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Bot.Models.Results;

/// <summary>
/// Results of a backtest or a live run.
/// </summary>
public class RunResults
{
    public List<DatedValue> UnderlyingValues = new();
    public List<DatedValue> PortfolioValues = new();
    public List<DatedValue> DailyReturns = new();
    public List<DatedValue> CumulativeReturns = new();
}

/// <summary>
/// Record that holds a timestamp and a value.
/// </summary>
public record DatedValue(DateTime Timestamp, decimal Value);

/// <summary>
/// Extensions for simplifying usage.
/// </summary>
public static class ResultExtensions
{
    public static void Add(this List<DatedValue> list, DateTime timestamp, decimal value)
    {
        list.Add(new DatedValue(timestamp, value));
    }
}

