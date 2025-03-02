using Bot.Brokers;
using Bot.Models;
using Bot.Strategies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.DataSources;
using Microsoft.Extensions.Logging;
using Bot.Models.Results;

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
    /// <param name="runMode"></param>
    /// <param name="interval"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    Task<RunResult> RunAsync(
        RunMode runMode,
        Interval interval,
        DateTime? start = null,
        DateTime? end = null);
}
