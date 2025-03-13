// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Allocations;
using Bot.Models.MarketData;

namespace Bot.Strategies.EpChan;

public class Ex3_4_BuyAndHold(string symbol) : StrategyBase
{
    private Allocation allocation = new Allocation { [symbol] = 1.0 };

    public override Allocation OnMarketData(MarketSnapshot snapshot)
    {
        return allocation;
    }
}
