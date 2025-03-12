// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Allocations;
using Bot.Models.MarketData;
using System;

namespace Bot.Strategies.EpChan;

public class Ex3_5_LongShort(string longSymbol, string shortSymbol) : StrategyBase
{
    private string longSymbol = longSymbol;
    private string shortSymbol = shortSymbol;

    public override Allocation OnMarketData(MarketSnapshot e)
    {
        throw new NotImplementedException();
    }
}
