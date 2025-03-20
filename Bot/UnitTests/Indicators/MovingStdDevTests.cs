// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Events;
using Bot.Exceptions;
using Bot.Helpers;
using Bot.Indicators;
using Bot.Models.MarketData;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.Indicators;

/*
public class MovingStdDevTests
{
    private static Random random = new Random();

    private MarketDataEvent CreateRandomMarketSnapshot(string symbol)
    {
        var bar = new Bar(
            DateTime.Now,
            symbol,
            random.NextDouble() * 100,
            random.NextDouble() * 100,
            random.NextDouble() * 100,
            random.NextDouble() * 100,
            random.Next(1000, 10000),
            random.NextDouble() * 100
        );

        return new MarketDataEvent(new MarketSnapshot(DateTime.Now, bar));
    }

    [Fact]
    public void MovingStdDev_NotHydrated_Throws()
    {
        int lookback = 5;
        var stdDev = new MovingStdDev(lookback, snapshot => snapshot["TEST"].AdjClose);

        var events = new List<MarketDataEvent>();
        for (int i = 0; i < lookback - 1; i++)
        {
            events.Add(CreateRandomMarketSnapshot("TEST"));
        }

        Assert.False(stdDev.IsHydrated);
        Assert.Throws<NotHydratedException>(() => stdDev.Value);
    }

    [Fact]
    public void MovingStdDev_CalculatesCorrectly()
    {
        int lookback = 5;
        var stdDev = new MovingStdDev(lookback, snapshot => snapshot["TEST"].AdjClose);

        var values = new List<double>();
        for (int i = 0; i < lookback; i++)
        {
            var @event = CreateRandomMarketSnapshot("TEST");
            values.Add(@event.Snapshot["TEST"].AdjClose);
            stdDev.OnMarketData(this, @event);
        }

        double expectedStdDev = MathFunctions.StdDev(values[^lookback..]);

        Assert.Equal(expectedStdDev, stdDev.Value, 4);
        Assert.True(stdDev.IsHydrated);
    }

    [Fact]
    public void MovingStdDev_HandlesMoreThanLookback()
    {
        int lookback = 5;
        var stdDev = new MovingStdDev(lookback, snapshot => snapshot["TEST"].AdjClose);

        var values = new List<double>();
        for (int i = 0; i < lookback + 2; i++)
        {
            var @event = CreateRandomMarketSnapshot("TEST");
            values.Add(@event.Snapshot["TEST"].AdjClose);
            stdDev.OnMarketData(this, @event);
        }

        double expectedStdDev = MathFunctions.StdDev(values[^lookback..]);

        Assert.Equal(expectedStdDev, stdDev.Value, 4);
        Assert.True(stdDev.IsHydrated);
    }
}
*/
