using Bot.Models.MarketData;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Bot.Models.Engine;

/// <summary>
/// Global configuration for the run.
/// </summary>
public class RunConfig
{
    public RunConfig(
        Interval interval = null,
        RunMode? runMode = null,
        DateTime? start = null,
        DateTime? end = null,
        Universe universe = null,
        bool shouldWriteCsv = false,
        [CallerFilePath] string callerFilePath = null)
    {
        Interval = interval ?? Interval.OneDay;
        RunMode = RunMode.BackTest;
        Start = start ?? DateTime.MinValue;
        End = end ?? DateTime.MaxValue;
        Universe = universe ?? throw new ArgumentNullException("Symbol universe must be defined.");
        ShouldWriteCsvOutput = shouldWriteCsv;

        string dateTimeStr = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
        string callerClass = Path.GetFileNameWithoutExtension(callerFilePath);
        CsvOutputFileName = $"{callerClass}.csv";
    }

    /// <summary>
    /// The interval of data we are running on.
    /// </summary>
    public Interval Interval { get; } 

    /// <summary>
    /// The mode to run in. Live, Paper, Backtest.
    /// </summary>
    public RunMode RunMode { get; }

    /// <summary>
    /// Start time (for backtest mode).
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// End time (for backtest mode).
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// The universer of stocks to run on.
    /// </summary>
    public Universe Universe { get; }

    /// <summary>
    /// Whether we should write all analyzer output to a csv at the end of the run.
    /// Mostly useful for debugging backtests.
    /// </summary>
    public bool ShouldWriteCsvOutput { get; }

    /// <summary>
    /// The file name if csv output is enabled.
    /// </summary>
    public string CsvOutputFileName { get; }
}
