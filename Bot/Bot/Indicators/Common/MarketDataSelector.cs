// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.MarketData;
using System;

namespace Bot.Indicators.Common;

public class MarketDataSelector<T_out>(Func<MarketSnapshot, T_out> selector, int lookback) 
    : IndicatorBase<MarketSnapshot, T_out>(lookback)
{
    protected Func<MarketSnapshot, T_out> _selector = selector;

    public override void OnAdd(MarketSnapshot input)
    {
        _value = _selector(input);
    }
}
