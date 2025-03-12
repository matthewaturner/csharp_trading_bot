// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Events;
using Bot.Models.Results;

namespace Bot.Analyzers;

public interface IStrategyAnalyzer : IMarketDataReceiver, IInitializeReceiver, IFinalizeReceiver
{
    public RunResult RunResult { get; }
}
