
namespace Bot.Indicators.Common;

/// <summary>
/// Exponential moving average indicator.
/// </summary>
public class ExponentialMovingAverage(int lookback) : IndicatorBase<double, double>(lookback)
{
    private readonly double _alpha = 2.0 / (lookback + 1);
    private bool _isFirst = true;

    public override void OnNext(double input)
    {
        if (_isFirst)
        {
            _value = input;
            _isFirst = false;
        }
        else
        {
            _value = (_alpha * input) + (1 - _alpha) * _value;
        }
    }
}

