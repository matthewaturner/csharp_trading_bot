
using Bot.Events;
using Bot.Models.Results;

namespace Bot.Analyzers;

public interface IStrategyAnalyzer : IMarketDataReceiver, IInitializeReceiver, IFinalizeReceiver
{
    public RunResult RunResult { get; }
}
