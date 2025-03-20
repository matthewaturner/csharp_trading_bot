using Bot.Indicators.Common;
using Bot.Models.MarketData;

namespace UnitTests.Indicators;

public class MarketDataIndicatorTests
{
    private readonly string Sym = "TEST";
    private readonly MarketSnapshot TestSnapshot = TestHelpers.CreateRandomSnapshot("TEST");

    [Fact]
    public void AdjClose()
    {
        var marketDataIndicator = MarketDataIndicator<double>.AdjClose(Sym);
        marketDataIndicator.Add(TestSnapshot);
        Assert.Equal(TestSnapshot[Sym].AdjClose, marketDataIndicator.Value);
        Assert.True(marketDataIndicator.IsHydrated);
    }
}
