using Bot.Brokers;
using Bot.DataSources.Interfaces;
using Bot.Engine.Events;
using Bot.Indicators;
using Bot.Logging;
using Bot.Models;
using Bot.Strategies;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Bot.Engine
{
    public class TradingEngine : ITradingEngine
    {
        /// <summary>
        /// Holds the current bars.
        /// </summary>
        private MultiBar bars;

        /// <summary>
        /// Objects that receive new bars as they come in.
        /// </summary>
        private IList<IBarReceiver> barReceivers;

        public TradingEngine()
        { 
            this.Logger = new ConsoleLogger(LogLevel.Information);
        }

        /// <summary>
        /// Get or set current bars.
        /// </summary>
        public MultiBar Bars => bars;

        /// <summary>
        /// Get or set all symbols in the universe.
        /// </summary>
        public IList<string> Symbols { get; set; }

        /// <summary>
        /// Get or set the broker object.
        /// </summary>
        public IBroker Broker { get; set; }

        /// <summary>
        /// Get or set the data source object.
        /// </summary>
        public IDataSource DataSource { get; set; }

        /// <summary>
        /// Get or set the strategy object.
        /// </summary>
        public IStrategy Strategy { get; set; }

        /// <summary>
        /// Gets or set the logger object.
        /// </summary>
        /// <value></value>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Get or set the output folder.
        /// </summary>
        public string OutputFolder { get; set; }

        /// <summary>
        /// Setup everything.
        /// </summary>
        private void Setup()
        {
            // initialize stuff
            bars = new MultiBar(Symbols.ToArray());

            Strategy.Initialize(this);
            Broker.Initialize(this);
            DataSource.Initialize(this);

            ClearReceivers();
            RegisterReceiver(DataSource);
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
                throw new NotImplementedException("Not tested.");

                // hydrate indicators
                /*
                await DataSource.StreamBars(
                    Symbols.ToArray(),
                    interval,
                    DateTime.UtcNow.GetNthPreviousTradingDay(Strategy.Lookback + 1),
                    null,
                    SendOnBarEvents);
                */

                // setup live streaming
            }
            else if (runMode == RunMode.BackTest)
            {
                await DataSource.StreamBars(
                    [.. Symbols],
                    interval,
                    start.Value,
                    end.Value,
                    SendOnBarEvents);
            }
        }

        /// <summary>
        /// Adds the objects to the receiver lists for the different types of events.
        /// </summary>
        /// <param name="obj"></param>
        private void RegisterReceiver(object obj)
        {
            if (obj is IBarReceiver barReceiver)
            {
                barReceivers.Add(barReceiver);
            }
        }

        /// <summary>
        /// Clears the receiver lists.
        /// </summary>
        private void ClearReceivers()
        {
            barReceivers = [];
        }

        /// <summary>
        /// Sends on bar events to all interested parties.
        /// </summary>
        /// <param name="bars"></param>
        private void SendOnBarEvents(Bar newBar)
        {
            Logger.LogInformation("-----");

            // update only the bar we received
            bars.Update(newBar);

            foreach (IBarReceiver receiver in barReceivers)
            {
                receiver.BaseOnBar(bars);
            }

            Logger.LogVerbose($"Account: {Broker.GetAccount()}");
        }
    }
}