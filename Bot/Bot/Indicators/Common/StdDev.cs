// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Helpers;
using System.Linq;

namespace Bot.Indicators.Common;

/// <summary>
/// There is a more efficient way to compute this than fully recalculating on a sliding window, 
/// but this is very simple and fine for now.
/// </summary>
public class StdDev(int lookback) : IndicatorBase<double, double>(lookback)
{
    private readonly SlidingWindowIndicator<double> _window = new SlidingWindowIndicator<double>(lookback);

    public override void OnNext(double input)
    {
        _window.Next(input);

        if (_window.IsHydrated)
        {
            _value = MathFunctions.StdDev(_window.Value.AsEnumerable());
        }
    }
}
