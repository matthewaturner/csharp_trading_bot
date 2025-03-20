// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Exceptions;
using Bot.Indicators;
using Bot.Indicators.Common;

namespace Bot.Tests.Indicators;

public class SimpleMovingAverageTests
{
    [Fact]
    public void Sma_Hydrates_After_Period()
    {
        var period = 3;
        var sma = new SimpleMovingAverage(period);

        Assert.False(sma.IsHydrated);

        sma.Add(1.0);
        Assert.False(sma.IsHydrated);

        sma.Add(2.0);
        Assert.False(sma.IsHydrated);

        sma.Add(3.0);
        Assert.True(sma.IsHydrated);

        Assert.Equal(2.0, sma.Value);
    }

    [Fact]
    public void Sma_Throws_If_Not_Hydrated()
    {
        var period = 3;
        var sma = new SimpleMovingAverage(period);

        sma.Add(1.0);
        Assert.False(sma.IsHydrated);

        Assert.Throws<NotHydratedException>(() => _ = sma.Value);
    }

    [Fact]
    public void Chained_Sma_Composition_Works()
    {
        var sma = new SimpleMovingAverage(3);
        var smaOfSquares = sma.Of(x => x*x);

        var inputs = new[] { 1.0, 2.0, 3.0, 4.0 };

        foreach (var input in inputs)
        {
            smaOfSquares.Add(input);
        }

        var expected = inputs[^3..].Select(i => i * i).Average();
        Assert.Equal(expected, smaOfSquares.Value);
    }
}
