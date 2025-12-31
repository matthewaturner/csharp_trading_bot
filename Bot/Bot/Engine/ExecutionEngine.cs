using Bot.Analyzers;
using Bot.Brokers;
using Bot.Events;
using Bot.Models.Allocations;
using Bot.Models.Broker;
using Bot.Models.Engine;
using Bot.Models.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Engine;

/// <summary>
/// Translates strategy allocations into actual trades and tracks realized performance.
/// Works for both backtesting (with simulated execution) and live trading.
/// </summary>
public class ExecutionEngine : IStrategyAnalyzer
{
    private ITradingEngine Engine;
    private IBroker Broker;
    private ILogger Logger;

    private double previousPortfolioValue;
    private double rebalanceThreshold;

    public RunResult RunResult { get; private set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="broker">The broker to use for order execution.</param>
    /// <param name="rebalanceThreshold">Minimum weight change (absolute) to trigger a trade. Default 0.01 (1%).</param>
    public ExecutionEngine(IBroker broker, double rebalanceThreshold = 0.01)
    {
        Broker = broker;
        this.rebalanceThreshold = rebalanceThreshold;
    }

    /// <summary>
    /// Handle initialize event.
    /// </summary>
    public void OnInitialize(object sender, EventArgs _)
    {
        Engine = sender as ITradingEngine;
        Logger = Engine.CreateLogger(nameof(ExecutionEngine));
        RunResult = new RunResult(Engine.RunConfig.Universe);
        previousPortfolioValue = Broker.GetPortfolio().PortfolioValue;
    }

    /// <summary>
    /// Handle market data event - rebalance portfolio and track performance.
    /// </summary>
    public void OnMarketData(object sender, MarketDataEvent e)
    {
        // 1. Get target allocation from strategies
        Allocation targetAllocation = Engine.MetaAllocation.FlattenAllocations();

        // 2. Get current positions from broker
        Allocation currentAllocation = GetCurrentAllocation();

        // 3. Calculate delta and execute rebalance
        Allocation delta = CalculateDelta(currentAllocation, targetAllocation);
        ExecuteRebalance(delta, e.Snapshot);

        // 4. Update portfolio value with current market data
        var portfolio = Broker.GetPortfolio();
        double currentPortfolioValue = portfolio.PortfolioValue;

        // 5. Calculate returns from actual portfolio value changes
        double portfolioReturn = previousPortfolioValue > 0
            ? (currentPortfolioValue - previousPortfolioValue) / previousPortfolioValue
            : 0;

        // 6. Track data for analysis
        RunResult.Timestamps.Add(e.Snapshot.Timestamp);
        RunResult.Returns.Add(portfolioReturn);

        // Get the actual allocation after orders are placed
        Allocation actualAllocation = GetCurrentAllocation();

        foreach (string symbol in Engine.RunConfig.Universe)
        {
            RunResult.UnderlyingPrices[symbol].Add(e.Snapshot[symbol].AdjClose);
            RunResult.SymbolWeights[symbol].Add(actualAllocation.GetAllocation(symbol));
        }

        previousPortfolioValue = currentPortfolioValue;
    }

    /// <summary>
    /// Calculate final metrics.
    /// </summary>
    public void OnFinalize(object sender, EventArgs _)
    {
        // For ExecutionEngine, we already have Returns calculated from actual P&L
        // We just need to calculate the derived metrics
        RunResult.CalculateResultsFromReturns(Engine.RunConfig.AnnualRiskFreeRate, Engine.RunConfig.Interval);
    }

    /// <summary>
    /// Get the current allocation from actual broker positions.
    /// </summary>
    private Allocation GetCurrentAllocation()
    {
        var portfolio = Broker.GetPortfolio();
        double totalValue = portfolio.PortfolioValue;

        if (totalValue <= 0)
        {
            return Allocation.Empty(Engine.RunConfig.Universe.ToArray());
        }

        var allocation = new Allocation();
        var positions = Broker.GetPositions();

        foreach (var position in positions)
        {
            if (position.Quantity == 0) continue;

            // Get current price from data source
            var currentBar = Engine.DataSource.GetLatestBar(position.Symbol);
            double positionValue = position.Quantity * currentBar.AdjClose;
            allocation[position.Symbol] = positionValue / totalValue;
        }

        // Fill in zeros for symbols not in positions
        foreach (string symbol in Engine.RunConfig.Universe)
        {
            if (!allocation.ContainsKey(symbol))
            {
                allocation[symbol] = 0;
            }
        }

        return allocation;
    }

    /// <summary>
    /// Calculate the difference between current and target allocations.
    /// </summary>
    private Allocation CalculateDelta(Allocation current, Allocation target)
    {
        var delta = new Allocation();
        var allSymbols = Engine.RunConfig.Universe;

        foreach (var symbol in allSymbols)
        {
            double currentWeight = current.GetAllocation(symbol);
            double targetWeight = target.GetAllocation(symbol);
            delta[symbol] = targetWeight - currentWeight;
        }

        return delta;
    }

    /// <summary>
    /// Execute rebalancing trades based on allocation delta.
    /// </summary>
    private void ExecuteRebalance(Allocation delta, Models.MarketData.MarketSnapshot snapshot)
    {
        var portfolio = Broker.GetPortfolio();
        double totalValue = portfolio.PortfolioValue;

        if (totalValue <= 0)
        {
            Logger?.LogWarning("Portfolio value is zero or negative. Skipping rebalance.");
            return;
        }

        foreach (var (symbol, weightDelta) in delta)
        {
            // Skip if the change is below the threshold
            if (Math.Abs(weightDelta) < rebalanceThreshold)
            {
                continue;
            }

            // Calculate dollar amount to trade
            double dollarAmount = weightDelta * totalValue;
            double currentPrice = snapshot[symbol].AdjClose;
            
            if (currentPrice <= 0)
            {
                Logger?.LogWarning($"Invalid price for {symbol}: {currentPrice}. Skipping.");
                continue;
            }

            // Calculate number of shares (rounded to whole shares)
            int shares = (int)Math.Round(dollarAmount / currentPrice);

            if (shares == 0)
            {
                continue;
            }

            // Determine order type based on direction
            OrderType orderType = shares > 0 ? OrderType.MarketBuy : OrderType.MarketSell;
            double quantity = Math.Abs(shares);

            var orderRequest = new OrderRequest(orderType, symbol, quantity);

            try
            {
                var order = Broker.PlaceOrder(orderRequest);
                Logger?.LogInformation($"Placed {orderType} order for {quantity} shares of {symbol}");
            }
            catch (Exception ex)
            {
                Logger?.LogError($"Failed to place order for {symbol}: {ex.Message}");
            }
        }
    }
}
