
using Bot.Indicators;
using Bot.Models.MarketData;

namespace UnitTests.Indicators;

public class MarketDataIndicatorTests
{
    private readonly string Sym = "TEST";
    private readonly MarketSnapshot TestSnapshot = TestHelpers.CreateRandomSnapshot("TEST");

    [Fact]
    public void AdjClose()
    {
        var marketDataIndicator = Ind.MarketData.AdjClose(Sym);
        marketDataIndicator.Next(TestSnapshot);
        Assert.Equal(TestSnapshot[Sym].AdjClose, marketDataIndicator.Value, 9);
        Assert.True(marketDataIndicator.IsHydrated);
    }

    [Fact]
    public void AdjClose_Composed()
    {
        var indicator = Ind.SMA(5).Of(Ind.MarketData.AdjClose(Sym));

        List<double> values = new List<double>();
        for (int i = 0; i < 10; i++)
        {
            var snapshot = TestHelpers.CreateRandomSnapshot(Sym);
            indicator.Next(snapshot);
            values.Add(snapshot[Sym].AdjClose);
        }

        double expectedValue = values[^5..].Average();

        Assert.Equal(expectedValue, indicator.Value, 9);
        Assert.True(indicator.IsHydrated);
    }
}
