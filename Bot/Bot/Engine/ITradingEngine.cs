using Bot.Brokers;
using Bot.DataSources;
using Bot.Models;
using Bot.Models.Results;
using Bot.Strategies;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Bot.Engine;

public interface ITradingEngine
{
    /// <summary>
    /// The single symbol that represents the universe currently
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// The interval we are trading on.
    /// </summary>
    public Interval Interval { get; }

    /// <summary>
    /// Gets current data source.
    /// </summary>
    public IDataSource DataSource { get; }

    /// <summary>
    /// Get current broker.
    /// </summary>
    public IBroker Broker { get; }

    /// <summary>
    /// Gets current strategy.
    /// </summary>
    public IStrategy Strategy { get; }

    /// <summary>
    /// Shared logger.
    /// </summary>
    public ILogger Logger { get; }

    /// <summary>
    /// Runs the strategy.
    /// </summary>
    Task<RunResult> RunAsync(
        RunMode runMode,
        Interval interval,
        DateTime? start = null,
        DateTime? end = null,
        string callerFilePath = null);
}
