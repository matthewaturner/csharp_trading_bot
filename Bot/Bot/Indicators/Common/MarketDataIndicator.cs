// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.MarketData;
using System;

namespace Bot.Indicators.Common;

public class MarketDataIndicator<T_out>(Func<MarketSnapshot, T_out> selector) 
    : FuncIndicator<MarketSnapshot, T_out>(selector, 1)
{ 
    public static MarketDataIndicator<double> Open(string symbol) => new(m => m[symbol].Open);
    public static MarketDataIndicator<double> High(string symbol) => new(m => m[symbol].High);
    public static MarketDataIndicator<double> Low(string symbol) => new(m => m[symbol].Low);
    public static MarketDataIndicator<double> Close(string symbol) => new(m => m[symbol].Close);
    public static MarketDataIndicator<long> Volume(string symbol) => new(m => m[symbol].Volume);
    public static MarketDataIndicator<double> AdjClose(string symbol) => new(m => m[symbol].AdjClose);
}
