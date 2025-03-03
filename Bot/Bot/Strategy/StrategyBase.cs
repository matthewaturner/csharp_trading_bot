
using Bot.Brokers;
using Bot.Engine;
using Bot.Events;
using Bot.Indicators;
using Bot.Models;
using Bot.Models.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Strategies;

public abstract class StrategyBase(List<IIndicator> indicators = null) : IStrategy
{
    public void Initialize(ITradingEngine engine)
    {
        Engine = engine;
    }

    public ITradingEngine Engine { get; private set; }

    public IList<IIndicator> Indicators { get; set; } = indicators ?? new List<IIndicator>();

    public int Lookback => Indicators.Max(ind => ind.Lookback);

    public bool IsHydrated => Indicators.All(ind => ind.IsHydrated);

    // Helpful proxies

    public IBroker Broker => Engine.Broker;

    public IAccount Account => Broker.GetAccount();

    /// <summary>
    /// The method that receives market data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void IMarketDataReceiver.OnMarketData(object sender, MarketDataEvent e)
    {
        GlobalConfig.GlobalLogger.LogInformation($"Received bar: {e.Bar}");
        ProcessBar(e.Bar);
    }

    public abstract void ProcessBar(Bar bar);
}
