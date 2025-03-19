// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Exceptions;
using Bot.Models.MarketData;
using System;
using System.Collections.Generic;

namespace Bot.Indicators;

public class SimpleMovingAverage : MarketDataIndicatorBase<double>
{
    private readonly Func<MarketSnapshot, double> _transform;
    private readonly Queue<double> _values;
    private double _value;

    public SimpleMovingAverage(
        int lookback,
        Func<MarketSnapshot, double> transform)
        : base(lookback)
    {
        _values = new Queue<double>(lookback);
        _value = 0;
        _transform = transform;
    }

    protected override double Iterate(MarketSnapshot input)
    {
        double newValue = _transform(input);
        _values.Enqueue(newValue);
        _value += newValue / _lookback;

        if (_values.Count > _lookback)
        {
            _value -= _values.Dequeue() / _lookback;
        }

        return _value;
    }

    public override double Value => IsHydrated ? _value : throw new NotHydratedException();
}
