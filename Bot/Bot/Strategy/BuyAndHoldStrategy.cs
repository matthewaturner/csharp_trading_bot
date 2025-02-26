using Bot.Models;
using Bot.Strategies;
using System;

namespace Bot.Strategy;

public class BuyAndHoldStrategy() : StrategyBase
{
    private bool invested = false;

    public override void ProcessBar(Bar bar)
    {
        if (!invested)
        {
            double funds = Engine.Broker.GetAccount().BuyingPower;
            Engine.Broker.PlaceOrder(new OrderRequest(OrderType.MarketBuy, bar.Symbol, funds / bar.Close, bar.Close));
            invested = true;
        }
    }
}
