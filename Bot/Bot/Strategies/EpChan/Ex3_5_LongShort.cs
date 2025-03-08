using Bot.Brokers;
using Bot.Models.MarketData;
using Bot.Strategies;

namespace Bot.Strategy.EpChan;

public class Ex3_5_LongShort(string longSymbol, string shortSymbol) : StrategyBase
{
    private string longSymbol = longSymbol;
    private string shortSymbol = shortSymbol;
    private bool invested = false;

    public override void OnMarketData(MarketSnapshot e)
    {
        if (!invested)
        {
            double cashPerLeg = Engine.Broker.GetAccount().Cash / 2;
            Engine.Broker.MarketBuy(longSymbol, cashPerLeg / e[longSymbol].AdjClose);
            Engine.Broker.MarketSell(shortSymbol, cashPerLeg / e[shortSymbol].AdjClose);
            invested = true;
        }
    }
}
