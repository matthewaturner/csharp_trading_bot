// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Bot.Indicators.Common;

public class StdDev : IndicatorBase<double, double>
{
    private readonly int _lookback;
    private readonly bool _useBesselCorrection;
    private readonly Queue<double> _window;

    private double _sum = 0.0;
    private double _sumOfSquares = 0.0;

    public StdDev(int lookback, bool useBesselCorrection = false) 
        : base(lookback)
    {
        if (lookback < 2)
            throw new ArgumentException("Lookback must be at least 2 for standard deviation", nameof(lookback));

        _lookback = lookback;
        _useBesselCorrection = useBesselCorrection;
        _window = new Queue<double>(lookback);
    }

    public override void OnNext(double input)
    {
        // Add input
        _window.Enqueue(input);

        // Remove oldest if window is full
        if (_window.Count > _lookback)
        {
            var oldest = _window.Dequeue();
            _sum -= oldest;
            _sumOfSquares -= oldest * oldest;
        }

        // Update sums
        _sum += input;
        _sumOfSquares += input * input;

        // Check hydration
        if (_window.Count < _lookback)
        {
            _value = double.NaN;
            return;
        }

        double mean = _sum / _lookback;
        double variance = (_sumOfSquares / _lookback) - (mean * mean);

        // Apply Bessel's correction (sample variance)
        if (_useBesselCorrection)
            variance *= (_lookback / (_lookback - 1.0));

        _value = Math.Sqrt(variance);
    }
}
