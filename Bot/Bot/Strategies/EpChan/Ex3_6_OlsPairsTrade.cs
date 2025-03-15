// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Allocations;
using Bot.Models.MarketData;
using System;

namespace Bot.Strategies.EpChan;

public class Ex3_6_OlsPairsTrade(
    string longSymbol, 
    string shortSymbol, 
    double hedgeRatio, 
    double spreadMean, 
    double spreadStdDev)
    : StrategyBase
{
    private Allocation longAllocation = new Allocation { [longSymbol] = 1, [shortSymbol] = -hedgeRatio };
    private Allocation shortAllocation = new Allocation { [longSymbol] = -1, [shortSymbol] = hedgeRatio };
    private Allocation neutralAllocation = new Allocation { [longSymbol] = 0, [shortSymbol] = 0 };
    private Allocation alloc = null;

    public override Allocation OnMarketData(MarketSnapshot bar)
    {
        double spread = bar[longSymbol].AdjClose - hedgeRatio * bar[shortSymbol].AdjClose;
        double z_score = (spread - spreadMean) / spreadStdDev;

        if (z_score > 2) 
            alloc = shortAllocation;
        else if (z_score < -2) 
            alloc = longAllocation;
        else if (alloc != neutralAllocation && Math.Abs(z_score) < 1)
            alloc = neutralAllocation;

        return alloc;
    }
}

