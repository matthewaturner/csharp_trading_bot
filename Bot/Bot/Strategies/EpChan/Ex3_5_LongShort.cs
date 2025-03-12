// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

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
            double targetExposure = Engine.Broker.GetPortfolio().Cash / 2;
            Engine.Broker.MarketBuy(longSymbol, targetExposure / e[longSymbol].AdjClose);
            Engine.Broker.MarketSell(shortSymbol, targetExposure / e[shortSymbol].AdjClose);

            invested = true;
        }
        else
        {
            var portfolio = Broker.GetPortfolio();
            double targetExposure = portfolio.GrossExposure / 2;

            // rebalance long position
            double longExposureDifference = targetExposure - portfolio.LongPositionsValue;
            if (longExposureDifference > 0)
            {
                Engine.Broker.MarketBuy(longSymbol, longExposureDifference / e[longSymbol].AdjClose);
            }
            else if (longExposureDifference < 0)
            {
                Engine.Broker.MarketSell(longSymbol, -longExposureDifference / e[longSymbol].AdjClose);
            }

            // rebalance short position
            double shortExposureDifference = targetExposure - portfolio.ShortPositionsValue;
            if (shortExposureDifference > 0)
            {
                Engine.Broker.MarketSell(shortSymbol, shortExposureDifference / e[shortSymbol].AdjClose);
            }
            else if (shortExposureDifference < 0)
            {
                Engine.Broker.MarketBuy(shortSymbol, -shortExposureDifference / e[shortSymbol].AdjClose);
            }
        }
    }
}
