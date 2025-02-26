using Bot.Brokers;
using Bot.Engine.Events;
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


namespace Bot.Engine;

public class TradingEngine() : ITradingEngine
{
    // Multi-bars object that holds the latest bar of each symbol in the universe.
    public MultiBar Bars { get; private set; }

    // All symbols in the universe
    public IList<string> Symbols { get; set; }

    // Broker object
    public IBroker Broker { get; set; } = new BackTestingBroker(10000);

    // Data source object
    public IDataSource DataSource { get; set; }

    // Single strategy object (for now)
    public IStrategy Strategy { get; set; }

    // Shared logger, todo should remove in favor of referencing from shared config always
    public ILogger Logger => GlobalConfig.Logger;

    /// <summary>
    /// Setup everything.
    /// </summary>
    private void Setup()
    {
        // initialize stuff
        Bars = new MultiBar(Symbols.ToArray());

        Broker.Initialize(this);
        Strategy.Initialize(this);

        RegisterReceiver(Broker);
        RegisterReceiver(Strategy);
    }

    /// <summary>
    /// Sets up the trading engine.
    /// </summary>
    public async Task RunAsync(
        RunMode runMode,
        Interval interval,
        DateTime? start = null,
        DateTime? end = null)
    {
        Setup();

        if (runMode == RunMode.Live || runMode == RunMode.Paper)
        {
            throw new NotImplementedException("Not implemented.");
        }
        else if (runMode == RunMode.BackTest)
        {
            await DataSource.StreamBars(
                [.. Symbols],
                interval,
                start.Value,
                end.Value);
        }
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
    }
}