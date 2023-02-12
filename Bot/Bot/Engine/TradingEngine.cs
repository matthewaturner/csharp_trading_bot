using Bot.Analyzers;
using Bot.Configuration;
using Bot.Engine.Events;
using Bot.Exceptions;
using Bot.Models;
using Bot.Strategies;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

using Bot.Data;
using System.IO;
using Bot.Brokers;
using Bot.Indicators;
using Bot.Analyzers.Loggers;

namespace Bot.Engine
{
    public class TradingEngine : ITradingEngine
    {
        private MultiTick ticks;

        private IList<ITickReceiver> tickReceivers;
        private IList<ITerminateReceiver> terminateReceivers;
        private IList<ILogReceiver> logReceivers;

        public TradingEngine()
        { }

        public IMultiTick Ticks { get; private set; }

        public IList<string> Symbols { get; set; }

        public IBroker Broker { get; set; }

        public IDataSource DataSource { get; set; }

        public IStrategy Strategy { get; set; }

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

            ClearReceiverLists();
            AddToReceiverLists(DataSource);
            AddToReceiverLists(Broker);
            AddToReceiverLists(Strategy);
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

                SendTerminateEvents();
            }
        }

        /// <summary>
        /// Adds the objects to the receiver lists for the different types of events.
        /// </summary>
        /// <param name="obj"></param>
        private void AddToReceiverLists(object obj)
        {
            if (obj is ITickReceiver tickReceiver)
            {
                tickReceivers.Add(tickReceiver);
            }

            if (obj is ITerminateReceiver terminateReceiver)
            {
                terminateReceivers.Add(terminateReceiver);
            }

            if (obj is ILogReceiver logReceiver)
            {
                logReceivers.Add(logReceiver);
            }
        }

        /// <summary>
        /// Clears the receiver lists.
        /// </summary>
        private void ClearReceiverLists()
        {
            tickReceivers = new List<ITickReceiver>();
            terminateReceivers = new List<ITerminateReceiver>();
            logReceivers = new List<ILogReceiver>();
        }

        /// <summary>
        /// Sends on tick events to all interested parties.
        /// </summary>
        /// <param name="ticks"></param>
        private void SendOnTickEvents(Tick[] newTicks)
        {
            ticks.Update(newTicks);
            foreach (ITickReceiver receiver in tickReceivers)
            {
                receiver.BaseOnTick(ticks);
            }
        }

        /// <summary>
        /// Sends finalize event to all interested parties.
        /// </summary>
        private void SendTerminateEvents()
        {
            foreach (ITerminateReceiver receiver in terminateReceivers)
            {
                receiver.OnTerminate();
            }
        }

        /// <summary>
        /// Sends logging events.
        /// </summary>
        /// <param name="log"></param>
        public void Log(string caller, string message, LogLevel level = LogLevel.Information )
        {
            foreach (ILogReceiver receiver in logReceivers)
            {
                receiver.OnLog(caller, message, level);
            }
        }
    }
}