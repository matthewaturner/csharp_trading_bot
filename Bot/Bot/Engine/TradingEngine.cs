using Bot.Brokers;
using Bot.Data.Interfaces;
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
        /// Holds the current ticks.
        /// </summary>
        private MultiTick ticks;

        /// <summary>
        /// Objects that receive new ticks as they come in.
        /// </summary>
        private IList<ITickReceiver> tickReceivers;

        public TradingEngine()
        { 
            this.Logger = new ConsoleLogger(LogLevel.Information);
        }

        /// <summary>
        /// Get or set current ticks.
        /// </summary>
        public IMultiTick Ticks => ticks;

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
        /// Validate that the engine is setup properly.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void ValidateConfiguration()
        {
            void ThrowIfNull(object o, string identifier)
            { 
                if (o == null)
                {
                    throw new ArgumentNullException(identifier);
                }
            }

            ThrowIfNull(Broker, nameof(Broker));
            ThrowIfNull(DataSource, nameof(DataSource));
            ThrowIfNull(Strategy, nameof(Strategy));
            ThrowIfNull(Symbols, nameof(Symbols));
            ThrowIfNull(ticks, nameof(ticks));
        }

        /// <summary>
        /// Setup everything.
        /// </summary>
        private void Setup()
        {
            // initialize stuff
            ticks = new MultiTick(Symbols.ToArray());

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
            TickInterval interval,
            DateTime? start = null,
            DateTime? end = null)
        {
            Setup();

            if (runMode == RunMode.Live || runMode == RunMode.Paper)
            {
                throw new NotImplementedException("Not tested.");

                // hydrate indicators
                await DataSource.StreamTicks(
                    Symbols.ToArray(),
                    interval,
                    DateTime.UtcNow.GetNthPreviousTradingDay(Strategy.Lookback + 1),
                    null,
                    SendOnTickEvents);

                // setup live streaming
            }
            else if (runMode == RunMode.BackTest)
            {
                await DataSource.StreamTicks(
                    Symbols.ToArray(),
                    interval,
                    start.Value,
                    end.Value,
                    SendOnTickEvents);
            }
        }

        /// <summary>
        /// Adds the objects to the receiver lists for the different types of events.
        /// </summary>
        /// <param name="obj"></param>
        private void RegisterReceiver(object obj)
        {
            if (obj is ITickReceiver tickReceiver)
            {
                tickReceivers.Add(tickReceiver);
            }
        }

        /// <summary>
        /// Clears the receiver lists.
        /// </summary>
        private void ClearReceivers()
        {
            tickReceivers = new List<ITickReceiver>();
        }

        /// <summary>
        /// Sends on tick events to all interested parties.
        /// </summary>
        /// <param name="ticks"></param>
        private void SendOnTickEvents(Tick newTick)
        {
            Logger.LogInformation("-----");

            // update only the tick we received
            ticks.Update(newTick);

            foreach (ITickReceiver receiver in tickReceivers)
            {
                receiver.BaseOnTick(ticks);
            }

            Logger.LogVerbose($"Account: {Broker.GetAccount()}");
        }
    }
}