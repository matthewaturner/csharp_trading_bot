// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.MarketData;

namespace UnitTests;

public static class TestHelpers
{
    private static readonly Random Rng = new Random();

    public static MarketSnapshot CreateRandomSnapshot(string symbol)
    {
        var bar = new Bar(
            DateTime.Now,
            symbol,
            Rng.NextDouble() * 100,
            Rng.NextDouble() * 100,
            Rng.NextDouble() * 100,
            Rng.NextDouble() * 100,
            Rng.Next(1000, 10000),
            Rng.NextDouble() * 100
        );

        return new MarketSnapshot(DateTime.Now, bar);
    }
}
