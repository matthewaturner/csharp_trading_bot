using Bot.Models;

namespace Bot.Events;

/// <summary>
/// The event to raise.
/// </summary>
public class MarketDataEvent(Bar bar)
{
    public Bar Bar { get; } = bar;
}

/// <summary>
/// Defines the method implemented by event receivers.
/// </summary>
public interface IMarketDataReceiver
{
    public void OnMarketData(object sender, MarketDataEvent e);
}

