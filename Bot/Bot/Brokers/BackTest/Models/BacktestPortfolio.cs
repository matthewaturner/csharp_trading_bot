
using Bot.Models.Interfaces;
using System;

namespace Bot.Brokers.BackTest.Models;

public class BacktestPortfolio : IPortfolio
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public BacktestPortfolio()
    {
        AccountId = "backtest-account";
        InitialCapital = 0;
        Cash = 0;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="initialCapital"></param>
    public BacktestPortfolio(double initialCapital)
    {
        AccountId = "backtest-account";
        Cash = initialCapital;
    }

    public string AccountId { get; set; }

    public double InitialCapital { get; private set; }

    public double Cash { get; private set; }

    public double PortfolioValue => Cash + LongPositionsValue - ShortPositionsValue;

    public double LongPositionsValue { get; private set; }

    public double ShortPositionsValue { get; private set; }

    public double GrossExposure => LongPositionsValue + Math.Abs(ShortPositionsValue);

    public double NetExposure => LongPositionsValue - Math.Abs(ShortPositionsValue);

    public double CapitalAtRisk => InitialCapital; // Or adjusted for margin

    public double Leverage => GrossExposure / CapitalAtRisk;

    public double RealizedPnL { get; private set; }

    public double UnrealizedPnL { get; private set; }
}
