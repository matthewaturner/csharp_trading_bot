using Bot.Events;
using Bot.Models.Results;

namespace Bot.Analyzers;

public interface IAnalyzer : IMarketDataReceiver, IInitialize, IFinalizeReceiver
{
    public RunResult RunResults { get; }
}
