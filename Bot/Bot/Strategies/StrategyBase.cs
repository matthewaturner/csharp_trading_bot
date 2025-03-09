
using Bot.Brokers;
using Bot.Engine;
using Bot.Events;
using Bot.Models.Broker;
using Bot.Models.MarketData;
using Microsoft.Extensions.Logging;
using System;

namespace Bot.Strategies;

public abstract class StrategyBase : IStrategy
{
    // private
    private ILogger logger;

    // public
    public ITradingEngine Engine { get; private set; }

    public IBroker Broker => Engine.Broker;

    public IPortfolio Account => Broker.GetPortfolio();

    /// <summary>
    /// Handle initialize event.
    /// </summary>
    public void OnInitialize(object sender, EventArgs _)
    {
        Engine = sender as ITradingEngine;
        this.logger = Engine.CreateLogger(this.GetType().Name);
    }

    /// <summary>
    /// Handler market data event.
    /// </summary>
    void IMarketDataReceiver.OnMarketData(object sender, MarketDataEvent e)
    {
        logger.LogDebug($"Received snapshot: {e.Snapshot}");
        OnMarketData(e.Snapshot);
    }

    public abstract void OnMarketData(MarketSnapshot bar);
}
