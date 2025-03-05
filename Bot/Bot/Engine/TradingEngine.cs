using Bot.Analyzers;
using Bot.Brokers;
using Bot.Brokers.BackTest;
using Bot.DataSources;
using Bot.DataSources.Alpaca;
using Bot.Helpers;
using Bot.Logging;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bot.Engine;

public class TradingEngine : ITradingEngine
{
    private ILoggerFactory loggerFactory;

    // Event handlers
    private event EventHandler<EventArgs> InitializeEventHandlers;
    private event EventHandler<EventArgs> FinalizeEventHandlers;

    // RunConfig object holds all the configuration for the run
    public RunConfig RunConfig { get; set;  }

    // Broker object
    public IBroker Broker { get; set; } = new BackTestingBroker(10000);

    // Data source object
    public IDataSource DataSource { get; set; } = new AlpacaDataSource();

    // Single analyzer for now
    public IStrategyAnalyzer Analyzer { get; set; } = new StrategyAnalyzer();

    // Single strategy object (for now)
    public IStrategy Strategy { get; set; }


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
        InitializeEventHandlers += Broker.OnInitialize;
        InitializeEventHandlers += Analyzer.OnInitialize;
        InitializeEventHandlers += Strategy.OnInitialize;

        // register market data handlers
        if (Broker is BackTestingBroker b)
        {
            DataSource.MarketDataReceivers += b.OnMarketData;
        }
        DataSource.MarketDataReceivers += Analyzer.OnMarketData;
        DataSource.MarketDataReceivers += Strategy.OnMarketData;

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
                    RunConfig.Universe.AllSymbols,
                    RunConfig.Interval,
                    RunConfig.Start,
                    RunConfig.End);
                break;
        }

        FinalizeEventHandlers?.Invoke(this, new EventArgs());

        if (RunConfig.ShouldWriteCsvOutput)
        {
            string fullFileName = Path.Join(GlobalConfig.OutputFolder, RunConfig.CsvOutputFileName);
            CsvExporter.ExportToCSV(Analyzer.RunResults, fullFileName);
        }

        return Analyzer.RunResults;
    }
}