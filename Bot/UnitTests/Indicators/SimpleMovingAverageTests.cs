using Bot.Events;
using Bot.Exceptions;
using Bot.Indicators;
using Bot.Models.MarketData;
using System;
using System.Collections.Generic;
using Xunit;

namespace Bot.Tests.Indicators;

public class SimpleMovingAverageTests
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
    public void SimpleMovingAverage_NotHydrated_Throws()
    {
        int lookback = 5;
        var sma = new SimpleMovingAverage(lookback, snapshot => snapshot["TEST"].AdjClose);

        var events = new List<MarketDataEvent>();
        for (int i = 0; i < lookback - 1; i++)
        {
            events.Add(CreateRandomMarketSnapshot("TEST"));
        }

        Assert.False(sma.IsHydrated);
        Assert.Throws<NotHydratedException>(() => sma.Value);
    }

    [Fact]
    public void SimpleMovingAverage_CalculatesCorrectly()
    {
        int lookback = 5;
        var sma = new SimpleMovingAverage(lookback, snapshot => snapshot["TEST"].AdjClose);

        var events = new List<MarketDataEvent>();
        for (int i = 0; i < lookback; i++)
        {
            events.Add(CreateRandomMarketSnapshot("TEST"));
        }

        double expectedSum = 0;
        foreach (var @event in events)
        {
            expectedSum += @event.Snapshot["TEST"].AdjClose;
            sma.OnMarketData(this, @event);
        }

        double expectedAverage = expectedSum / lookback;
        Assert.Equal(expectedAverage, sma.Value, 4);
        Assert.True(sma.IsHydrated);
    }

    [Fact]
    public void SimpleMovingAverage_HandlesMoreThanLookback()
    {
        int lookback = 5;
        var sma = new SimpleMovingAverage(lookback, snapshot => snapshot["TEST"].AdjClose);

        var events = new List<MarketDataEvent>();
        for (int i = 0; i < lookback + 2; i++)
        {
            events.Add(CreateRandomMarketSnapshot("TEST"));
        }

        double expectedSum = 0;
        for (int i = 0; i < lookback; i++)
        {
            expectedSum += events[i].Snapshot["TEST"].AdjClose;
            sma.OnMarketData(this, events[i]);
        }

        for (int i = lookback; i < events.Count; i++)
        {
            expectedSum += events[i].Snapshot["TEST"].AdjClose;
            expectedSum -= events[i - lookback].Snapshot["TEST"].AdjClose;
            sma.OnMarketData(this, events[i]);
        }

        double expectedAverage = expectedSum / lookback;
        Assert.Equal(expectedAverage, sma.Value, 4);
        Assert.True(sma.IsHydrated);
    }
}
