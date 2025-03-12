// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Brokers;
using Bot.DataSources;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Bot.Engine;

public interface ITradingEngine
{
    /// <summary>
    /// The config for this run.
    /// </summary>
    public RunConfig RunConfig { get; }

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
    /// Create a logger for use in a downstream class.
    /// </summary>
    public ILogger CreateLogger(string name);

    /// <summary>
    /// Runs the strategy.
    /// </summary>
    Task<RunResult> RunAsync();
}
