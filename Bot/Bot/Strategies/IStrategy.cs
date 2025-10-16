
using Bot.Events;
using Bot.Models.Allocations;
using Bot.Models.MarketData;

namespace Bot.Strategies;

public interface IStrategy : IInitializeReceiver
{
    string Id { get; }

    public int Lookback { get; }

    Allocation OnMarketDataBase(MarketSnapshot snapshot);
}
