
using Bot.Indicators.Interfaces;
using Bot.Models;
using System;

namespace Bot.Indicators;

public class SimpleMovingAverage : IndicatorBase<double>, ISimpleValueIndicator<double>
{
    private readonly Func<MultiBar, double> transform;
    private double[] data;
    private double average;
    private int index;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="lookback"></param>
    /// <param name="transform"></param>
    public SimpleMovingAverage(int lookback, Func<MultiBar, double> transform)
        : base(lookback)
    {
        if (lookback < 1)
        {
            throw new ArgumentException("Lookback must be >= 1.");
        }

        this.transform = transform;
        data = new double[lookback];
        index = 0;
        average = 0;
    }

    public override string Name => $"SMA-{Lookback}";

    public double Value => average;

    /// <summary>
    /// Calculates new values.
    /// </summary>
    /// <param name="bars"></param>
    public override void OnBar(MultiBar bars)
    {
        average = average - (data[index] / Lookback);
        data[index] = transform(bars);
        average = average + (data[index] / Lookback);

        index = (index + 1) % Lookback;

        if (!IsHydrated && index == 0)
        {
            IsHydrated = true;
        }
    }

    public override string ToString()
    {
        return $"{Name} = {Value}";
    }
}
