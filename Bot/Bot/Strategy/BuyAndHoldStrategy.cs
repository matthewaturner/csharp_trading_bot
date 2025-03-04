using Bot.Brokers;
using Bot.Models.MarketData;
using Bot.Strategies;

namespace Bot.Strategy;

public class BuyAndHoldStrategy(string symbol) : StrategyBase
{
    private string symbol = symbol;
    private bool invested = false;

    public override void ProcessBar(MarketSnapshot snapshot)
    {
        if (!invested)
        {
            Broker.MarketBuy(symbol, Account.BuyingPower / snapshot[symbol].AdjClose);
            invested = true;
        }
    }
}
