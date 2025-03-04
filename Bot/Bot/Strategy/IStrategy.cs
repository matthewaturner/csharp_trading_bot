using Bot.Events;
using Bot.Models.MarketData;

namespace Bot.Strategies;

public interface IStrategy : IInitialize, IMarketDataReceiver
{
    void ProcessBar(MarketSnapshot snapshot);
}
