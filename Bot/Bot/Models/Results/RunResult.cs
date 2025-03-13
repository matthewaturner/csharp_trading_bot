// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

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
    public List<string> SymbolUniverse { get; private set; }

    public RunResult(List<string> symbolUniverse)
    {
        SymbolUniverse = symbolUniverse;

        foreach (string symbol in symbolUniverse)
        {
            UnderlyingPrices[symbol] = new List<double>();
            SymbolWeights[symbol] = new List<double>();
        }
    }

    // ####################################################
    // # Below values are set by the analyzer             #
    // ####################################################

    public List<DateTime> Timestamps = new();

    // list per symbol in the universe
    public Dictionary<string, List<double>> UnderlyingPrices = new();

    // weights per symbol in the universe
    public Dictionary<string, List<double>> SymbolWeights = new();

    // ####################################################
    // # Below values are calculated                      #
    // ####################################################

    public Dictionary<string, List<double>> UnderlyingReturns = new();

    // overall returns of the strategy
    public List<double> Returns { get; set; }

    public List<double> CumulativeReturns { get; set; }

    public List<double> ExcessReturns { get; set; }

    public List<double> HighWaterMark { get; set; }

    public List<double> Drawdown { get; set; }

    public List<double> DrawdownDuration { get; set; }

    public double AnnualizedSharpeRatio = double.NaN;
    public double MaximumDrawdown = 0;
    public double MaximumDrawdownDuration = 0;

    /// <summary>
    /// Calculate all the values.
    /// </summary>
    public void CalculateResults(double annualRiskFreeRate, Interval interval)
    {
        // returns must start at the right length but all zeros
        Returns = Enumerable.Repeat(0.0, UnderlyingPrices.Values.First().Count()).ToList();

        foreach (string symbol in SymbolUniverse)
        {
            UnderlyingReturns[symbol] = [.. UnderlyingPrices[symbol].Zip(UnderlyingPrices[symbol].Skip(1), (prev, current) => (current - prev) / prev)];
            var symbolReturns = SymbolWeights[symbol].Zip(UnderlyingReturns[symbol], (w, r) => w * r);

            // add the symbol returns to the strategy returns
            Returns = [.. Returns.Zip(symbolReturns, (r, sr) => r + sr)];
        }

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
        foreach (string symbol in SymbolUniverse) { UnderlyingReturns[symbol].Insert(0, 0); }
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
