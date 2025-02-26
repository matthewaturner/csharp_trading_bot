using Bot.Models;

namespace Bot.Events;

public class MarketDataEvent(Bar bar)
{
    public Bar Bar { get; } = bar;
}

