// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Allocations;
using Bot.Models.MarketData;
using System;

namespace Bot.Strategies.EpChan;

public class Ex3_4_BuyAndHold : StrategyBase
{
    private Allocation alloc;

    public Ex3_4_BuyAndHold(string symbol)
    {
        this.alloc = new Allocation();
        this.alloc.SetAllocation(symbol, 1.0);
    }

    public override Allocation OnMarketData(MarketSnapshot snapshot)
    {
        return alloc;
    }
}
