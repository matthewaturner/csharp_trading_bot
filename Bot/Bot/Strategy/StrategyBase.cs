
using Bot.Events;
using Bot.Indicators;
using Bot.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Strategies;

public abstract class StrategyBase : IStrategy, IMarketDataReceiver
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public StrategyBase()
    {
        Indicators = new List<IIndicator>();
    }

    public int Lookback => Indicators.Max(ind => ind.Lookback);

    public bool IsHydrated => Indicators.All(ind => ind.IsHydrated);

    public IList<IIndicator> Indicators { get; set; }

    /// <summary>
    /// The method that receives market data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnMarketData(object sender, MarketDataEvent e)
    {
        GlobalConfig.Logger.LogInformation($"Received bar: {e.Bar}");
    }

    public abstract void ProcessBar(Bar bar);
}
