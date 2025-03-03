using Bot.Brokers;
using Bot.Models;
using Bot.Strategies;
using System.Threading.Tasks;
using System;
using Bot.DataSources;
using Microsoft.Extensions.Logging;
using Bot.Brokers.BackTest;
using Bot.Events;
using Bot.DataSources.Alpaca;
using Bot.Analyzers;
using Bot.Models.Results;
using System.IO;
using System.Runtime.CompilerServices;
using Bot.Helpers;

namespace Bot.Engine;

public class TradingEngine(bool writeCsvOutput = false) : ITradingEngine
{
    private bool WriteCsvOutput = writeCsvOutput;

    #region Shared Properties ========================================================================================

    // The single symbol that currently represents the universe of stocks
    public string Symbol { get; set; }

    // The interval we are running on, daily hourly monthly etc.
    public Interval Interval { get; private set; }

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

    #endregion

    #region Event Handlers ===========================================================================================

    private event EventHandler<FinalizeEvent> FinalizeEvent;

    #endregion

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
    public async Task<RunResult> RunAsync(
        RunMode runMode,
        Interval interval,
        DateTime? start = null,
        DateTime? end = null,
        [CallerFilePath] string callerFilePath = null)
    {
        Interval = interval;
        Setup();

        switch (runMode)
        {
            default:
            case RunMode.Live:
            case RunMode.Paper:
                throw new NotImplementedException("Not implemented.");

            case RunMode.BackTest:
                await DataSource.StreamBars(
                    Symbol,
                    interval,
                    start ?? DateTime.MinValue,
                    end ?? DateTime.MaxValue);
                break;
        }

        FinalizeEvent?.Invoke(this, new FinalizeEvent());

        if (!string.IsNullOrWhiteSpace(callerFilePath) && WriteCsvOutput)
        {
            string dateTimeStr = DateTime.Now.ToString("yyyy-MM-ddTHH-mm-ss");
            string callerClass = Path.GetFileNameWithoutExtension(callerFilePath);
            string fullFileName = Path.Join(GlobalConfig.OutputFolder, $"{dateTimeStr} {callerClass}.csv");
            CsvExporter.ExportToCSV(Analyzer.RunResults, fullFileName);
        }

        return Analyzer.RunResults;
    }
}