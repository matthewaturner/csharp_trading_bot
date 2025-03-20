// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Helpers;
using System.Linq;

namespace Bot.Indicators.Common;

public class MovingStdDev(int lookback) : IndicatorBase<double, double>(lookback)
{
    private readonly SlidingWindowIndicator<double> _window = new SlidingWindowIndicator<double>(lookback);

    public override void OnAdd(double input)
    {
        _window.OnAdd(input);

        if (_window.IsHydrated)
        {
            _value = MathFunctions.StdDev(_window.Value.AsEnumerable());
        }
    }
}
