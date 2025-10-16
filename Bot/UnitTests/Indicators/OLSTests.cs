
using Bot.Exceptions;
using Bot.Helpers;
using Bot.Indicators;

namespace UnitTests.Indicators;

public class OLSTests
{
    readonly string SYM1 = "TEST1";
    readonly string SYM2 = "TEST2";

    [Fact]
    public void MovingOLS_NotHydrated_Throws()
    {
        int lookback = 5;
        var ols = Ind.OLS(lookback).Of(Ind.MarketData.Pair(SYM1, SYM2));

        for (int i = 0; i < lookback - 1; i++)
        {
            ols.Next(TestHelpers.CreateRandomSnapshot(SYM1, SYM2));
        }

        Assert.False(ols.IsHydrated);
        Assert.Throws<NotHydratedException>(() => ols.Value);
    }

    [Fact]
    public void MovingOLS_CalculatesCorrectly()
    {
        int lookback = 5;
        var ols = Ind.OLS(lookback).Of(Ind.MarketData.Pair(SYM1, SYM2));

        var values = new List<(double, double)>();
        for (int i = 0; i < lookback; i++)
        {
            var snapshot = TestHelpers.CreateRandomSnapshot(SYM1, SYM2);
            ols.Next(snapshot);
            values.Add((snapshot[SYM1].AdjClose, snapshot[SYM2].AdjClose));
        }

        double expected = MathFunctions.OLS(
            values.Select(v => v.Item1).ToArray(), 
            values.Select(v => v.Item2).ToArray());

        Assert.Equal(expected, ols.Value, 9);
        Assert.True(ols.IsHydrated);
    }

    [Fact]
    public void MovingOLS_HandlesMoreThanLookback()
    {
        int lookback = 5;
        var ols = Ind.OLS(lookback).Of(Ind.MarketData.Pair(SYM1, SYM2));

        var values = new List<(double, double)>();
        for (int i = 0; i < lookback + 2; i++)
        {
            var snapshot = TestHelpers.CreateRandomSnapshot(SYM1, SYM2);
            ols.Next(snapshot);
            values.Add((snapshot[SYM1].AdjClose, snapshot[SYM2].AdjClose));
        }

        double expected = MathFunctions.OLS(
            values.Select(v => v.Item1).ToArray()[^lookback..], 
            values.Select(v => v.Item2).ToArray()[^lookback..]);

        Assert.Equal(expected, ols.Value, 9);
        Assert.True(ols.IsHydrated);
    }
}
