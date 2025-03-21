// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Indicators.Common;
using Bot.Models.MarketData;
using System;

namespace Bot.Indicators;

/// <summary>
/// Static shorthand helper for creating new indicators.
/// </summary>
public static class Ind
{
    public static class MarketData
    {
        public static MarketDataIndicator<double> Open(string symbol) => new(s => s[symbol].Open);
        public static MarketDataIndicator<double> High(string symbol) => new(s => s[symbol].High);
        public static MarketDataIndicator<double> Low(string symbol) => new(s => s[symbol].Low);
        public static MarketDataIndicator<double> Close(string symbol) => new(s => s[symbol].Close);
        public static MarketDataIndicator<long> Volume(string symbol) => new(s => s[symbol].Volume);
        public static MarketDataIndicator<double> AdjClose(string symbol) => new(s => s[symbol].AdjClose);

        // get the entire snapshot
        public static MarketDataIndicator<MarketSnapshot> All => new(m => m);
        // get a pair of data points
        public static MarketDataIndicator<(double, double)> Pair(string symbol1, string symbol2) 
            => new(s => (s[symbol1].AdjClose, s[symbol2].AdjClose));
    }

    public static SimpleMovingAverage SMA(int lookback) => new SimpleMovingAverage(lookback);

    public static ExponentialMovingAverage EMA(int lookback) => new ExponentialMovingAverage(lookback);

    public static StdDev StdDev(int lookback) => new StdDev(lookback);

    public static OLSIndicator OLS(int lookback) => new OLSIndicator(lookback);

    public static SlidingWindowIndicator<double> PriceWindow(int lookback) => new SlidingWindowIndicator<double>(lookback);

    public static FuncIndicator<T_in, T_out> Func<T_in, T_out>(Func<T_in, T_out> f) => new FuncIndicator<T_in, T_out>(f);
}
