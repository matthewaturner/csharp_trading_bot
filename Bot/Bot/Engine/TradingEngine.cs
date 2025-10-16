
using Bot.Analyzers;
using Bot.DataSources;
using Bot.Events;
using Bot.Helpers;
using Bot.Logging;
using Bot.Models.Allocations;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bot.Engine;

public partial class TradingEngine : ITradingEngine, IMarketDataReceiver
{
    private ILoggerFactory loggerFactory;

    // Event handlers
    private event EventHandler<EventArgs> InitializeEventHandlers;
    private event EventHandler<EventArgs> FinalizeEventHandlers;

    // Local variables
    public RunConfig RunConfig { get; private set; }
    public MetaAllocation MetaAllocation { get; private set; }
    public StrategyAnalyzer Analyzer { get; private set; }
    public IDataSource DataSource { get; private set; }
    public IStrategy Strategy { get; private set; }

    /// <summary>
    /// Method to create loggers.
    /// </summary>
    public ILogger CreateLogger(string name) => new ColoredLogger(loggerFactory.CreateLogger(name));

    /// <summary>
    /// Setup everything.
    /// </summary>
    private void Setup()
    {
        // create logger factory
        loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "[HH:mm:ss] ";
                    options.SingleLine = true;
                    options.ColorBehavior = LoggerColorBehavior.Enabled;
                })
                .AddConsole()
                .SetMinimumLevel(RunConfig.LogLevel);
        });

        // register initialize handlers
        InitializeEventHandlers += Analyzer.OnInitialize;
        InitializeEventHandlers += Strategy.OnInitialize;

        // register market data handlers
        DataSource.MarketDataReceivers += this.OnMarketData;
        DataSource.MarketDataReceivers += Analyzer.OnMarketData;

        // register finalize handlers
        FinalizeEventHandlers += Analyzer.OnFinalize;

        // send initialize event
        InitializeEventHandlers?.Invoke(this, new EventArgs());
    }

    /// <summary>
    /// Sets up the trading engine.
    /// </summary>
    public async Task<RunResult> RunAsync()
    {
        Setup();

        switch (RunConfig.RunMode)
        {
            default:
            case RunMode.Live:
            case RunMode.Paper:
                throw new NotImplementedException("Not implemented.");

            case RunMode.BackTest:
                await DataSource.StreamBars(
                    RunConfig.Universe,
                    RunConfig.Interval,
                    RunConfig.Start,
                    RunConfig.End);
                break;
        }

        FinalizeEventHandlers?.Invoke(this, new EventArgs());

        if (RunConfig.ShouldWriteCsvOutput)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
            string fullFileName = Path.Join(GlobalConfig.OutputFolder, $"{timestamp} {RunConfig.CsvOutputFileName}");
            CsvExporter.ExportToCSV(Analyzer.RunResult, fullFileName);
        }

        return Analyzer.RunResult;
    }

    /// <summary>
    /// Handle market data and send it along to the strategy classes.
    /// </summary>
    public void OnMarketData(object sender, MarketDataEvent e)
    {
        var allocation = Strategy.OnMarketDataBase(e.Snapshot);
        MetaAllocation.SetStrategyAllocation(Strategy.Id, allocation);
    }
}