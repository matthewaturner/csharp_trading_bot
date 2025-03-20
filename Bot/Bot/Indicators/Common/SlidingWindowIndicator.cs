// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System.Collections.Generic;

namespace Bot.Indicators.Common;

public class SlidingWindowIndicator<T>(int lookback)
    : IndicatorBase<T, IReadOnlyList<T>>(lookback)
{
    private readonly Queue<T> _window = new Queue<T>(lookback);
    private readonly int _lookback = lookback;

    public override void OnNext(T input)
    {
        _window.Enqueue(input);
        if (_window.Count == _lookback + 1) _window.Dequeue();

        _value = _window.ToArray();
    }
}
