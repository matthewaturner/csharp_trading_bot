// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Events;
using Bot.Models.MarketData;

namespace Bot.Strategies;

public interface IStrategy : IInitializeReceiver, IMarketDataReceiver
{
    void OnMarketData(MarketSnapshot snapshot);
}
