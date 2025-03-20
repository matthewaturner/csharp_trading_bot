// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Exceptions;
using Bot.Indicators;

namespace UnitTests.Indicators;

public class SimpleMovingAverageTests
{
    [Fact]
    public void Sma_Hydrates_After_Period()
    {
        var period = 3;
        var sma = Ind.SMA(period);

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
        var sma = Ind.SMA(period);
        sma.Add(1.0);

        Assert.False(sma.IsHydrated);
        Assert.Throws<NotHydratedException>(() => _ = sma.Value);
    }

    [Fact]
    public void Sma_of_squares()
    {
        var smaOfSquares = Ind.SMA(3).Of(x => x*x);

        var inputs = new[] { 1.0, 2.0, 3.0, 4.0 };

        foreach (var input in inputs)
        {
            smaOfSquares.Add(input);
        }

        var expected = inputs[^3..].Select(i => i * i).Average();
        Assert.Equal(expected, smaOfSquares.Value, 9);
    }

    [Fact]
    public void Squares_of_sma()
    {
        var squaresOfSma = Ind.Func((double x) => x*x).Of(Ind.SMA(3));

        var inputs = new[] { 1.0, 2.0, 3.0, 4.0 };

        foreach (var input in inputs)
        {
            squaresOfSma.Add(input);
        }

        var avg = inputs[^3..].Average();
        Assert.Equal(avg*avg, squaresOfSma.Value, 9);
    }
}
