// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Indicators.Common;

namespace UnitTests.Indicators;

public class EmaTests
{
    [Fact]
    public void Ema_Should_Not_Be_Hydrated_Initially()
    {
        var ema = new ExponentialMovingAverage(5);

        Assert.False(ema.IsHydrated);

        // Hydrated after lookback number of inputs
        for (int i = 0; i < 4; i++)
        {
            ema.Next(1.0);
            Assert.False(ema.IsHydrated);
        }

        // Should hydrate on the 5th input
        ema.Next(1.0);
        Assert.True(ema.IsHydrated);
    }

    [Fact]
    public void Ema_Calculates_Correctly_Simple_Case()
    {
        var ema = new ExponentialMovingAverage(2); // alpha = 2 / (2 + 1) = 0.666...

        // First value initializes the EMA, not hydrated yet
        ema.Next(10.0);
        Assert.False(ema.IsHydrated);

        // Second value hydrates it
        ema.Next(20.0);
        Assert.True(ema.IsHydrated);

        // EMA = 0.666 * 20 + 0.333 * 10 = 16.666...
        Assert.Equal(16.666666, ema.Value, 4);

        // Third value
        ema.Next(30.0);
        // EMA = 0.666 * 30 + 0.333 * 16.666 = 25.555...
        Assert.Equal(25.555555, ema.Value, 4);
    }

    [Fact]
    public void Ema_Weights_Decrease_Over_Time()
    {
        var ema = new ExponentialMovingAverage(3); // alpha = 2 / (3 + 1) = 0.5

        ema.Next(50); // first value, not hydrated yet
        Assert.False(ema.IsHydrated);

        ema.Next(50); // second value, still not hydrated
        Assert.False(ema.IsHydrated);

        ema.Next(50); // third value, hydrated now
        Assert.True(ema.IsHydrated);

        // EMA = 50/2 + 50/2 = 50
        Assert.Equal(50, ema.Value, 4);

        // EMA = 100/2 + 50/2 = 75
        ema.Next(100); 
        Assert.Equal(75, ema.Value, 4);

        // EMA = 25/2 + 75/2 = 50
        ema.Next(25);
        Assert.Equal(50, ema.Value, 4);
    }

    [Fact]
    public void Ema_Hydration_Depends_On_Lookback()
    {
        var ema = new ExponentialMovingAverage(3);
        Assert.False(ema.IsHydrated);

        ema.Next(10);
        Assert.False(ema.IsHydrated);

        ema.Next(20);
        Assert.False(ema.IsHydrated);

        ema.Next(30);
        Assert.True(ema.IsHydrated);
    }
}

