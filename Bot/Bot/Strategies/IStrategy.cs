// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Events;
using Bot.Models.Allocations;
using Bot.Models.MarketData;
using System;

namespace Bot.Strategies;

public interface IStrategy : IInitializeReceiver
{
    string Id { get; }

    Allocation OnMarketDataBase(MarketSnapshot snapshot);
}
