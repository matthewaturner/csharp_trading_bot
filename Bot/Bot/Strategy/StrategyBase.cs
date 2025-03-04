
using Bot.Brokers;
using Bot.Engine;
using Bot.Events;
using Bot.Models.Interfaces;
using Bot.Models.MarketData;
using Microsoft.Extensions.Logging;

namespace Bot.Strategies;

public abstract class StrategyBase() : IStrategy
{
    public void Initialize(ITradingEngine engine)
    {
        Engine = engine;
    }

    public ITradingEngine Engine { get; private set; }

    public IBroker Broker => Engine.Broker;

    public IAccount Account => Broker.GetAccount();

    /// <summary>
    /// The method that receives market data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void IEventReceiver<MarketDataEvent>.OnEvent(object sender, MarketDataEvent e)
    {
        GlobalConfig.GlobalLogger.LogInformation($"Received snapshot: {e.Snapshot}");
        ProcessBar(e.Snapshot);
    }

    public abstract void ProcessBar(MarketSnapshot bar);
}
