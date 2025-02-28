using Bot.Events;

namespace Bot.Analyzers;

public interface IAnalyzer : IMarketDataReceiver, IInitialize
{
}
