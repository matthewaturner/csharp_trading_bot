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


namespace Bot.Engine
{
    public class TradingEngine : ITradingEngine
    {
        private MultiBar bars;

        private ILogger _logger = GlobalConfig.Logger;

        /// <summary>
        /// Objects that receive new bars as they come in.
        /// </summary>
        private IList<IBarReceiver> barReceivers;

        public TradingEngine()
        { }

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
                throw new NotImplementedException("Not implemented.");
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
            _logger.LogInformation("-----");

            // update only the bar we received
            bars.Update(newBar);

            foreach (IBarReceiver receiver in barReceivers)
            {
                receiver.BaseOnBar(bars);
            }

            _logger.LogDebug($"Account: {Broker.GetAccount()}");
        }
    }
}