// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Allocations;
using Bot.Models.MarketData;

namespace Bot.Strategies.EpChan;

public class Ex3_5_LongShort(string longSymbol, string shortSymbol) : StrategyBase
{
    Allocation allocation = new Allocation { [longSymbol] = 0.5, [shortSymbol] = -.5 };

    public override Allocation OnMarketData(MarketSnapshot e)
    {
        return allocation;
    }
}
