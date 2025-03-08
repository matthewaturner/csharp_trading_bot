﻿using Bot.Brokers.Backtest.Models;
using Bot.Events;
using Bot.Models;
using Bot.Models.Broker;
using Bot.Models.MarketData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Brokers.Backtest;

public class BacktestBroker : BrokerBase, IBroker, IMarketDataReceiver
{
    // private objects
    private BacktestPortfolio account;
    private IList<BacktestOrder> openOrders;
    private ExecutionMode executionMode;

    private Bar CurrentBar(string symbol) => Engine.DataSource.GetLatestBar(symbol);

    /// <summary>
    /// Dependency injection constructor.
    /// </summary>
    public BacktestBroker(double initialFunds, ExecutionMode executionMode = ExecutionMode.OnCurrentBar)
    {
        this.account = new BacktestPortfolio(initialFunds);
        this.openOrders = new List<BacktestOrder>();
        this.executionMode = executionMode;
    }

    /// <summary>
    /// Execute orders at the next price.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnMarketData(object sender, MarketDataEvent e)
    {
        if (executionMode == ExecutionMode.OnNextBar)
        {
            ExecuteOrders();
        }
    }

    /// <summary>
    /// Gets asset information.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public IAssetInformation GetAssetInformation(string symbol)
    {
        return new BacktestAssetInformation(symbol);
    }

    /// <summary>
    /// Gets account object.
    /// </summary>
    public IPortfolio GetPortfolio()
    {
        return account;
    }

    /// <summary>
    /// Gets all positions held.
    /// </summary>
    public IList<IPosition> GetPositions()
    {
        return account.Positions.Values.ToList<IPosition>();
    }

    /// <summary>
    /// Gets the position held in some symbol.
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public IPosition GetPosition(string symbol)
    {
        return account.Positions[symbol] ?? new BacktestPosition(symbol, 0);
    }

    /// <summary>
    /// Gets all outstanding orders.
    /// </summary>
    /// <returns></returns>
    public IList<IOrder> GetOpenOrders()
    {
        return openOrders.ToList<IOrder>();
    }

    /// <summary>
    /// Gets the status of an order.
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public IOrder GetOrder(string orderId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets all orders.
    /// </summary>
    /// <returns></returns>
    public IList<IOrder> GetAllOrders()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets all orders in some state (open, filled, etc.)
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public IList<IOrder> QueryOrders(IEnumerable<string> symbols, OrderState state, DateTime after, DateTime until, int limit = 50)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Placing an order just puts it into the open orders list.
    /// </summary>
    /// <param name="order"></param>
    public IOrder PlaceOrder(IOrderRequest request)
    {
        logger.LogInformation($"Placed order type: {request.Type} for {request.Quantity} quantity of {request.Symbol}");

        BacktestOrder order = new BacktestOrder(request);
        order.OrderId = Guid.NewGuid().ToString();
        order.PlacementTime = DateTime.Now;
        openOrders.Add(order);

        if (executionMode == ExecutionMode.OnCurrentBar)
        {
            order.AverageFillPrice = DataSource.GetLatestBar(request.Symbol).AdjClose;
            ExecuteOrders();
        }

        return order;
    }

    /// <summary>
    /// Cancels an order if it hasn't been filled yet.
    /// </summary>
    /// <param name="orderId"></param>
    public void CancelOrder(string orderId)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Execute all of the outstanding orders at the current bar price.
    /// </summary>
    /// <param name="bar"></param>
    private void ExecuteOrders()
    {
        foreach (var order in openOrders)
        {
            account.ApplyOrder(order);
        }
    }
}
