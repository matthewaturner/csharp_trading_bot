using Bot.Events;
using Bot.Models.Results;

namespace Bot.Engine;

/// <summary>
/// Interface for execution engines that translate strategy allocations into trades
/// and track performance through actual portfolio value changes.
/// </summary>
public interface IExecutionEngine : IMarketDataReceiver, IInitializeReceiver, IFinalizeReceiver
{
    /// <summary>
    /// Results of the backtest or live run.
    /// </summary>
    public RunResult RunResult { get; }
}
