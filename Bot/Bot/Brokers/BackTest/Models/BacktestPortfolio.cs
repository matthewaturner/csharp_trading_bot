// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------


using Bot.Brokers.BackTest.Models;
using Bot.Models.Broker;
using Bot.Models.MarketData;
using Bot.Models.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Brokers.Backtest.Models;

public class BacktestPortfolio : IPortfolio
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public BacktestPortfolio()
    {
        AccountId = "backtest-account";
        Positions = new();
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
        Positions = new();
        Cash = initialCapital;
    }

    public string AccountId { get; set; }

    public Dictionary<string, BacktestPosition> Positions { get; private set; }

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

    /// <summary>
    /// Update all the values of the portfolio based on the snapshot prices.
    /// </summary>
    public void ApplyMarketData(MarketSnapshot snapshot)
    {
        LongPositionsValue = Positions.Values.Where(p => p.Quantity > 0).Sum(p => p.Quantity * snapshot[p.Symbol].AdjClose);
        ShortPositionsValue = Positions.Values.Where(p => p.Quantity < 0).Sum(p => Math.Abs(p.Quantity) * snapshot[p.Symbol].AdjClose);
    }

    /// <summary>
    /// Apply a transaction for a given symbol to the portfolio.
    /// </summary>
    public void ApplyOrder(BacktestOrder order, double fillPrice)
    {
        order.AverageFillPrice = fillPrice;

        double transactionValue = order.Quantity * fillPrice;

        if (!Positions.ContainsKey(order.Symbol))
        {
            Positions[order.Symbol] = new BacktestPosition(order.Symbol, 0);
        }

        // todo: this should work for now but it's wrong in general,
        // the decision to add the transaction to the positions value should also take into account changes to current positions
        switch (order.Type)
        {
            case OrderType.MarketBuy:
                Cash -= transactionValue;
                LongPositionsValue += transactionValue;
                Positions[order.Symbol].Quantity += order.Quantity;
                break;
            case OrderType.MarketSell:
                Cash += transactionValue;
                ShortPositionsValue += transactionValue;
                Positions[order.Symbol].Quantity -= order.Quantity;
                break;
            default:
                throw new NotImplementedException($"Order type {order.Type} not implemented in ApplyOrder.");
        }

        order.State = OrderState.Filled;
    }

    /// <summary>
    /// Get the snapshot values of this portfolio.
    /// </summary>
    public PortfolioSnapshot GetSnapshot(DateTime timestamp)
    {
        return new PortfolioSnapshot()
        {
            Timestamp = timestamp,
            Cash = Cash,
            PortfolioValue = PortfolioValue,
            LongPositionsValue = LongPositionsValue,
            ShortPositionsValue = ShortPositionsValue,
            GrossExposure = GrossExposure,
            NetExposure = NetExposure,
            CapitalAtRisk = CapitalAtRisk,
            Leverage = Leverage,
            RealizedPnL = RealizedPnL,
            UnrealizedPnL = UnrealizedPnL
        };
    }
}
