
using Bot.Models.Allocations;
using Bot.Models.MarketData;
using System;

namespace Bot.Strategies.EpChan;

public class Ex3_6_OlsPairsTrade : StrategyBase
{
    private readonly string longSymbol;
    private readonly string shortSymbol;
    private readonly double spreadMean;
    private readonly double spreadStdDev;
    private readonly double hedgeRatio;

    private Allocation longAllocation;
    private Allocation shortAllocation;
    private Allocation neutralAllocation;
    private Allocation alloc = null;

    public Ex3_6_OlsPairsTrade(
        string longSymbol, 
        string shortSymbol, 
        double hedgeRatio, 
        double spreadMean, 
        double spreadStdDev)
    {
        this.longSymbol = longSymbol;
        this.shortSymbol = shortSymbol;
        this.spreadMean = spreadMean;
        this.spreadStdDev = spreadStdDev;
        this.hedgeRatio = hedgeRatio;

        this.longAllocation = GetUnitPortfolioLongToShort(longSymbol, shortSymbol, hedgeRatio);
        this.shortAllocation = -longAllocation;
        this.neutralAllocation = Allocation.Empty(longSymbol, shortSymbol);
    }

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

    private static Allocation GetUnitPortfolioLongToShort(
        string longSymbol,
        string shortSymbol,
        double hedgeRatio)
    {
        double totalWeight = 1 + Math.Abs(hedgeRatio);
        return new Allocation { [longSymbol] = 1 / totalWeight, [shortSymbol] = -hedgeRatio / totalWeight };
    }

}

