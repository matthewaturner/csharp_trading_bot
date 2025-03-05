using Bot.Events;
using Bot.Models.MarketData;

namespace Bot.Strategies;

public interface IStrategy : IInitializeReceiver, IMarketDataReceiver
{
    void ProcessBar(MarketSnapshot snapshot);
}
