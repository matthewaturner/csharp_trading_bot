// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Brokers;
using Bot.Models.MarketData;
using Bot.Strategies;

namespace Bot.Strategy.EpChan;

public class Ex3_4_BuyAndHold(string symbol) : StrategyBase
{
    private string symbol = symbol;
    private bool invested = false;

    public override void OnMarketData(MarketSnapshot snapshot)
    {
        if (!invested)
        {
            Broker.MarketBuy(symbol, Account.Cash / snapshot[symbol].AdjClose);
            invested = true;
        }
    }
}
