
using System.Collections.Generic;

namespace Bot.Indicators.Common;

public class SimpleMovingAverage(int lookback) 
    : IndicatorBase<double, double>(lookback)
{
    private readonly int _lookback = lookback;
    private readonly Queue<double> _inputData = new Queue<double>(lookback);

    public override void OnNext(double input)
    {
        _inputData.Enqueue(input);
        _value += input / _lookback;

        if (_inputData.Count > _lookback)
        {
            _value -= _inputData.Dequeue() / _lookback;
        }
    }
}
