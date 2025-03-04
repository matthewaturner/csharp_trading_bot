using Bot.Analyzers;
using Bot.Brokers;
using Bot.Brokers.BackTest;
using Bot.DataSources;
using Bot.DataSources.Alpaca;
using Bot.Events;
using Bot.Helpers;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bot.Engine;

public class TradingEngine : ITradingEngine
{
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

    // Shared logger, todo should remove in favor of referencing from shared config always
    public ILogger Logger => GlobalConfig.GlobalLogger;

    /// <summary>
    /// Finalize event handlers.
    /// </summary>
    private event EventHandler<FinalizeEvent> FinalizeEvent;

    /// <summary>
    /// Setup everything.
    /// </summary>
    private void Setup()
    {
        // initialize stuff
        Broker.Initialize(this);
        Analyzer.Initialize(this);
        Strategy.Initialize(this);

        // register market data receivers
        if (Broker is IMarketDataReceiver b)
        {
            DataSource.MarketDataReceivers += b.OnEvent;
        }
        DataSource.MarketDataReceivers += Analyzer.OnEvent;
        DataSource.MarketDataReceivers += Strategy.OnEvent;

        // register finalize receivers
        FinalizeEvent += Analyzer.OnFinalize;
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

        FinalizeEvent?.Invoke(this, new FinalizeEvent());

        if (RunConfig.ShouldWriteCsvOutput)
        {
            string fullFileName = Path.Join(GlobalConfig.OutputFolder, RunConfig.CsvOutputFileName);
            CsvExporter.ExportToCSV(Analyzer.RunResults, fullFileName);
        }

        return Analyzer.RunResults;
    }
}