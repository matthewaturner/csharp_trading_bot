﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.Results;

/// <summary>
/// Results of a backtest or a live run.
/// </summary>
public class RunResult
{
    public List<DatedValue> UnderlyingValues = new();
    public List<DatedValue> PortfolioValues = new();
    public List<DatedValue> DailyReturns = new();
    public List<DatedValue> CumulativeReturns = new();

    public double SharpeRatio = double.NaN;
    public double MaxDrawdown = double.NaN;
    public double MaxDrawdownDuration = double.NaN;
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
    public static void Add(this List<DatedValue> list, DateTime timestamp, decimal value)
    {
        list.Add(new DatedValue(timestamp, (double)value));
    }

    public static void Add(this List<DatedValue> list, DateTime timestamp, double value)
    {
        list.Add(new DatedValue(timestamp, value));
    }

    public static IEnumerable<double> ToDoubles(this List<DatedValue> list)
    {
        return list.Select(v => (double)v.Value);
    }
}

