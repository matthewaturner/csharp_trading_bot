using Bot.Brokers;
using Bot.Models;
using Bot.Strategies;

namespace Bot.Strategy;

public class BuyAndHoldStrategy() : StrategyBase
{
    private bool invested = false;

    public override void ProcessBar(Bar bar)
    {
        if (!invested)
        {
            Broker.MarketBuy(bar.Symbol, Account.BuyingPower / bar.AdjClose);
            invested = true;
        }
    }
}
