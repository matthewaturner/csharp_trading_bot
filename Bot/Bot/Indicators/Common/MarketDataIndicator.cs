
using Bot.Models.MarketData;
using System;

namespace Bot.Indicators.Common;

public class MarketDataIndicator<T_out>(Func<MarketSnapshot, T_out> selector) 
    : FuncIndicator<MarketSnapshot, T_out>(selector, 1)
{ 
}
