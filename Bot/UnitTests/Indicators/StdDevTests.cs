
using Bot.Exceptions;
using Bot.Helpers;
using Bot.Indicators;

namespace UnitTests.Indicators;

public class StdDevTests
{
    [Fact]
    public void MovingStdDev_NotHydrated_Throws()
    {
        int lookback = 5;
        var stdDev = Ind.StdDev(lookback).Of(Ind.MarketData.AdjClose("TEST"));

        for (int i = 0; i < lookback - 1; i++)
        {
            stdDev.Next(TestHelpers.CreateRandomSnapshot("TEST"));
        }

        Assert.False(stdDev.IsHydrated);
        Assert.Throws<NotHydratedException>(() => stdDev.Value);
    }

    [Fact]
    public void MovingStdDev_CalculatesCorrectly()
    {
        int lookback = 5;
        var stdDev = Ind.StdDev(lookback).Of(Ind.MarketData.AdjClose("TEST"));

        var values = new List<double>();
        for (int i = 0; i < lookback; i++)
        {
            var snapshot = TestHelpers.CreateRandomSnapshot("TEST");
            stdDev.Next(snapshot);
            values.Add(snapshot["TEST"].AdjClose);
        }

        double expectedStdDev = MathFunctions.StdDev(values, false);

        Assert.Equal(expectedStdDev, stdDev.Value, 9);
        Assert.True(stdDev.IsHydrated);
    }

    [Fact]
    public void MovingStdDev_HandlesMoreThanLookback()
    {
        int lookback = 5;
        var stdDev = Ind.StdDev(lookback).Of(Ind.MarketData.AdjClose("TEST"));

        var values = new List<double>();
        for (int i = 0; i < lookback + 7; i++)
        {
            var snapshot = TestHelpers.CreateRandomSnapshot("TEST");
            stdDev.Next(snapshot);
            values.Add(snapshot["TEST"].AdjClose);
        }

        double expectedStdDev = MathFunctions.StdDev(values[^lookback..], false);

        Assert.Equal(expectedStdDev, stdDev.Value, 9);
        Assert.True(stdDev.IsHydrated);
    }
}
