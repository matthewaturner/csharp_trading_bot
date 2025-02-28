using Bot.Brokers;
using Bot.Engine;
using Bot.Events;
using Bot.Models.Results;
using System;
using System.Linq;

namespace Bot.Analyzers;

public class StrategyAnalyzer : IAnalyzer
{
    private ITradingEngine Engine;

    private IBroker Broker => Engine.Broker;

    // the object we are defining
    public RunResults RunResults = new();

    public void Initialize(ITradingEngine engine)
    {
        Engine = engine;
    }

    /// <summary>
    /// Update the RunResults every time we receive market data.
    /// </summary>
    public void OnMarketData(object sender, MarketDataEvent e)
    {
        DateTime currentTime = e.Bar.Timestamp;

        decimal currentPortfolioValue = Broker.GetAccount().TotalValue;
        decimal previousPortfolioValue = RunResults.PortfolioValues.LastOrDefault()?.Value ?? currentPortfolioValue;
        RunResults.PortfolioValues.Add(currentTime, currentPortfolioValue);

        decimal dailyReturn = (currentPortfolioValue - previousPortfolioValue) / previousPortfolioValue;
        RunResults.DailyReturns.Add(currentTime, dailyReturn);

        decimal prevCumulativeReturn = RunResults.CumulativeReturns.LastOrDefault()?.Value ?? 0;
        decimal cumulativeReturn = (1 + prevCumulativeReturn) * (1 + dailyReturn) - 1;
        RunResults.CumulativeReturns.Add(currentTime, cumulativeReturn);
    }
}
