// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;

namespace Bot.Indicators;

public class FuncIndicator<T_in, T_out>(Func<T_in, T_out> func, int lookback = 1) 
    : IndicatorBase<T_in, T_out>(lookback), IIndicator<T_in, T_out>
{
    private readonly Func<T_in, T_out> _func = func;

    public override void OnNext(T_in input)
    {
        _value = _func(input);
    }
}
