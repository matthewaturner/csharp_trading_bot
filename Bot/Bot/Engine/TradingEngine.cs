using Bot.Brokers;
using Bot.Models;
using Bot.Strategies;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using Bot.DataSources;
using Microsoft.Extensions.Logging;
using Bot.Brokers.BackTest;
using Bot.Events;
using Bot.DataSources.Alpaca;
using Bot.Analyzers;
using Bot.Models.Results;


namespace Bot.Engine;

public class TradingEngine() : ITradingEngine
{
    #region Shared Properties ========================================================================================

    // Multi-bars object that holds the latest bar of each symbol in the universe.
    public MultiBar Bars { get; private set; }

    // All symbols in the universe
    public IList<string> Symbols { get; set; }

    // The interval we are running on, daily hourly monthly etc.
    public Interval Interval { get; private set; }

    // Broker object
    public IBroker Broker { get; set; } = new BackTestingBroker(10000);

    // Data source object
    public IDataSource DataSource { get; set; } = new AlpacaDataSource();

    // Single analyzer for now
    public IAnalyzer Analyzer { get; set; } = new StrategyAnalyzer();

    // Single strategy object (for now)
    public IStrategy Strategy { get; set; }

    // Shared logger, todo should remove in favor of referencing from shared config always
    public ILogger Logger => GlobalConfig.Logger;

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
        Bars = new MultiBar(Symbols.ToArray());

        Broker.Initialize(this);
        Analyzer.Initialize(this);
        Strategy.Initialize(this);

        // register market data receivers
        if (Broker is IMarketDataReceiver b)
        {
            DataSource.MarketDataReceivers += b.OnMarketData;
        }
        DataSource.MarketDataReceivers += Analyzer.OnMarketData;
        DataSource.MarketDataReceivers += Strategy.OnMarketData;

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
        DateTime? end = null)
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
                    [.. Symbols],
                    interval,
                    start ?? DateTime.MinValue,
                    end ?? DateTime.MaxValue);
                break;

        }

        FinalizeEvent?.Invoke(this, new FinalizeEvent());
        return Analyzer.RunResults;
    }

    /// <summary>
    /// Adds the objects to the receiver lists for the different types of events.
    /// </summary>
    /// <param name="obj"></param>
    private void RegisterReceiver(object obj)
    {
        if (obj is IMarketDataReceiver marketDataReceiver)
        {
            DataSource.MarketDataReceivers += marketDataReceiver.OnMarketData;
        }

        if (obj is IFinalizeReceiver finalizeReceiver)
        {
            FinalizeEvent += finalizeReceiver.OnFinalize;
        }
    }
}