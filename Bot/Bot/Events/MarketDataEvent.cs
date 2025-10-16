
using Bot.Models.MarketData;

namespace Bot.Events;

/// <summary>
/// An event containing new market data.
/// </summary>
/// <param name="snapshot"></param>
public class MarketDataEvent(MarketSnapshot snapshot)
{
    public MarketSnapshot Snapshot { get; } = snapshot;
}

