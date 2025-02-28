using Bot.Models;
using Bot.Indicators.Interfaces;
using System;

namespace Bot.Indicators;

public class MovingAverageCrossover : IndicatorBase<PositionType>, IMovingAverageCrossover
{
    private SimpleMovingAverage shortMA;
    private SimpleMovingAverage longMA;
    private PositionType position;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="shortLookback"></param>
    /// <param name="longLookback"></param>
    /// <param name="transform"></param>
    public MovingAverageCrossover(
        int shortLookback,
        int longLookback,
        Func<MultiBar, decimal> transform)
        : base(longLookback)
    {
        if (shortLookback >= longLookback)
        {
            throw new ArgumentException("shortLookback must be <= longLookback.");
        }

        shortMA = new SimpleMovingAverage(shortLookback, transform);
        longMA = new SimpleMovingAverage(longLookback, transform);
        Lookback = longLookback;
        position = PositionType.Neutral;
    }

    public override string Name => $"MAC-{Lookback}";

    public PositionType Value => position;

    public decimal ShortMa => shortMA.Value;

    public decimal LongMa => longMA.Value;

    /// <summary>
    /// Calculate new values.
    /// </summary>
    /// <param name="bars"></param>
    public override void OnBar(MultiBar bars)
    {
        shortMA.OnBar(bars);
        longMA.OnBar(bars);
        IsHydrated = shortMA.IsHydrated && longMA.IsHydrated;

        if (IsHydrated)
        {
            position = shortMA.Value > longMA.Value ? PositionType.Long : PositionType.Short;
        }
    }

    public override string ToString()
    {
        return $"{Name} = {Value}";
    }
}
