
using Bot.Indicators;
using Bot.Models.Allocations;
using Bot.Models.MarketData;
using System;
using System.Linq;

namespace Bot.Strategies.Custom;

public class OlsPairsTradeStrategy : StrategyBase
{
    private readonly string symbol1;
    private readonly string symbol2;

    IIndicator<MarketSnapshot, double> smoothOls;
    IIndicator<MarketSnapshot, double[]> priceWindow1;
    IIndicator<MarketSnapshot, double[]> priceWindow2;

    Allocation currentAlloc = null;

    public OlsPairsTradeStrategy(string longSymbol, string shortSymbol, int lookback)
    {
        this.symbol1 = longSymbol;
        this.symbol2 = shortSymbol;

        var ols = Ind.OLS(lookback).Of(Ind.MarketData.Pair(shortSymbol, longSymbol));
        smoothOls = Ind.EMA(3).Of(ols);

        priceWindow1 = Ind.PriceWindow(lookback).Of(Ind.MarketData.AdjClose(longSymbol));
        priceWindow2 = Ind.PriceWindow(lookback).Of(Ind.MarketData.AdjClose(shortSymbol));

        currentAlloc = Allocation.Empty(symbol1, symbol2);
    }

    public override Allocation OnMarketData(MarketSnapshot bar)
    {
        smoothOls.Next(bar);
        priceWindow1.Next(bar);
        priceWindow2.Next(bar);

        if (!smoothOls.IsHydrated || !priceWindow1.IsHydrated || !priceWindow2.IsHydrated)
        {
            return Allocation.Empty(symbol1, symbol2);
        }

        double hedgeRatio = smoothOls.Value;

        double[] prices1 = priceWindow1.Value;
        double[] prices2 = priceWindow2.Value;
        double[] spread = prices1.Zip(prices2, (p1, p2) => p1 - hedgeRatio * p2).ToArray();

        double spreadMean = spread.Average();
        double spreadStdDev = Math.Sqrt(spread.Sum(x => Math.Pow(x - spreadMean, 2)) / (spread.Length - 1));
        double zScore = (spread.Last() - spreadMean) / spreadStdDev;

        if (zScore > 1.8)
        {
            currentAlloc = new Allocation() { [symbol1] = -1, [symbol2] = hedgeRatio };
            currentAlloc.ToUnitPortfolio();
        }
        else if (zScore < -1.8)
        {
            currentAlloc = new Allocation() { [symbol1] = 1, [symbol2] = -hedgeRatio };
            currentAlloc.ToUnitPortfolio();
        }
        else if (zScore > -1.5 && zScore < 1.5)
        {
            currentAlloc = Allocation.Empty(symbol1, symbol2);
        }

        return currentAlloc;
    }
}
