// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.MarketData;

namespace UnitTests;

public static class TestHelpers
{
    private static readonly Random Rng = new Random();

    public static Bar CreateRandomBar(string symbol)
    {
        return new Bar(
            DateTime.Now,
            symbol,
            Rng.NextDouble() * 100,
            Rng.NextDouble() * 100,
            Rng.NextDouble() * 100,
            Rng.NextDouble() * 100,
            Rng.Next(1000, 10000),
            Rng.NextDouble() * 100);
    }

    public static MarketSnapshot CreateRandomSnapshot(params string[] symbols)
    {
        var bars = symbols.Select(CreateRandomBar).ToArray();
        return new MarketSnapshot(DateTime.Now, bars);
    }
}
