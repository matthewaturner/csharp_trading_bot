
using System.Collections.Generic;

namespace Bot.Indicators.Common;

/// <summary>
/// Indicator that calculates the ordinary least squares error (OLS) to find 
/// the hedge ratio for a pair of assets.
/// </summary>
public class OLSIndicator(int lookback) : IndicatorBase<(double X, double Y), double>(lookback)
{
    private readonly Queue<(double X, double Y)> _window = new(lookback);
    private readonly int _lookback = lookback;

    private double _sumXY = 0.0;
    private double _sumX2 = 0.0;

    public override void OnNext((double X, double Y) input)
    {
        _window.Enqueue(input);
        _sumXY += input.X * input.Y;
        _sumX2 += input.X * input.X;

        if (_window.Count == _lookback + 1)
        {
            (double X, double Y) = _window.Dequeue();
            _sumXY -= X * Y;
            _sumX2 -= X * X;
        }

        _value = _sumXY / _sumX2;
    }
}
