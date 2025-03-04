using Bot.Models.MarketData;

namespace Bot.Events;

/// <summary>
/// The event to raise.
/// </summary>
public class MarketDataEvent(MarketSnapshot bars)
{
    public MarketSnapshot Snapshot { get; } = bars;
}

/// <summary>
/// Defines the method implemented by event receivers.
/// </summary>
public interface IMarketDataReceiver : IEventReceiver<MarketDataEvent>
{
}

