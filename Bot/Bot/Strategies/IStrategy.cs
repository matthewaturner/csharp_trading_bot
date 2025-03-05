using Bot.Events;
using Bot.Models.MarketData;

namespace Bot.Strategies;

public interface IStrategy : IInitializeReceiver, IMarketDataReceiver
{
    void OnMarketData(MarketSnapshot snapshot);
}
