// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Indicators.Common;
using System;

namespace Bot.Indicators;

/// <summary>
/// Static shorthand helper for creating new indicators.
/// </summary>
public static class Ind
{
    public static class MarketData
    {
        public static MarketDataIndicator<double> Open(string symbol) => MarketDataIndicator<double>.Open(symbol);
        public static MarketDataIndicator<double> High(string symbol) => MarketDataIndicator<double>.High(symbol);
        public static MarketDataIndicator<double> Low(string symbol) => MarketDataIndicator<double>.Low(symbol);
        public static MarketDataIndicator<double> Close(string symbol) => MarketDataIndicator<double>.Close(symbol);
        public static MarketDataIndicator<long> Volume(string symbol) => MarketDataIndicator<long>.Volume(symbol);
        public static MarketDataIndicator<double> AdjClose(string symbol) => MarketDataIndicator<double>.AdjClose(symbol);
    }

    public static SimpleMovingAverage SMA(int lookback) => new SimpleMovingAverage(lookback);

    public static StdDev StdDev(int lookback) => new StdDev(lookback);

    public static FuncIndicator<T_in, T_out> Func<T_in, T_out>(Func<T_in, T_out> f) => new FuncIndicator<T_in, T_out>(f);
}
