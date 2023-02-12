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

using static Bot.Program;
using Bot.Data;
using System.IO;
using Bot.Brokers;
using Bot.Indicators;
using Bot.Analyzers.Loggers;

namespace Bot.Engine
{
    public class TradingEngine : ITradingEngine
    {
        private readonly DataSourceResolver dataSourceResolver;
        private readonly BrokerResolver brokerResolver;
        private readonly StrategyResolver strategyResolver;
        private readonly AnalyzerResolver analyzerResolver;

        private EngineConfig config;
        private MultiTick ticks;
        private IList<string> symbols;

        private IList<ITickReceiver> tickReceivers;
        private IList<ITerminateReceiver> terminateReceivers;
        private IList<ILogReceiver> logReceivers;

        private IBroker broker;
        private IDataSource dataSource;
        private IStrategy strategy;
        private IList<IAnalyzer> analyzers;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataSourceResolver"></param>
        /// <param name="brokerResolver"></param>
        /// <param name="strategyResolver"></param>
        /// <param name="analyzerResolver"></param>
        public TradingEngine(
            DataSourceResolver dataSourceResolver,
            BrokerResolver brokerResolver,
            StrategyResolver strategyResolver,
            AnalyzerResolver analyzerResolver)
        {
            this.dataSourceResolver = dataSourceResolver;
            this.brokerResolver = brokerResolver;
            this.strategyResolver = strategyResolver;
            this.analyzerResolver = analyzerResolver;

            tickReceivers = new List<ITickReceiver>();
            terminateReceivers = new List<ITerminateReceiver>();
            logReceivers = new List<ILogReceiver>();
        }

        /// <summary>
        /// Current symbols.
        /// </summary>
        public IList<string> Symbols => symbols;

        /// <summary>
        /// Current ticks.
        /// </summary>
        public IMultiBar Ticks => ticks;

        /// <summary>
        /// Current broker.
        /// </summary>
        public IBroker Broker => broker;

        /// <summary>
        /// Current datasource.
        /// </summary>
        public IDataSource DataSource => dataSource;

        /// <summary>
        /// Current strategy.
        /// </summary>
        public IStrategy Strategy => strategy;

        /// <summary>
        /// Current analyzers.
        /// </summary>
        public IList<IAnalyzer> Analyzers => analyzers;

        /// <summary>
        /// Output path for logging.
        /// </summary>
        public string OutputPath { get; private set; }

        /// <summary>
        /// Setup everything.
        /// </summary>
        public void Initialize(
            IStrategy strategy, 
            EngineConfig config, 
            IDataSource dataSource, 
            IBroker broker, 
            string outputPath)
        {
            this.config = config;
            ClearReceiverLists();

            // setup path for output
            OutputPath = outputPath;

            // initialize stuff
            ticks = new MultiTick(config.Symbols.ToArray());
            symbols = config.Symbols;

            // data source
            dataSource = dataSourceResolver(config.DataSource.Name);
            dataSource.Initialize(this, config.DataSource.Args);
            AddToReceiverLists(dataSource);

            // broker - needs to come before strategy
            broker = brokerResolver(config.Broker.Name);
            broker.Initialize(this, config.RunMode, config.Broker.Args);
            AddToReceiverLists(broker);

            // strategy
            strategy = strategyResolver(config.Strategy.Name);
            strategy.Initialize(this, config.Strategy.Args);
            AddToReceiverLists(strategy);

            // analyzers
            analyzers = new List<IAnalyzer>();
            foreach (DependencyConfig analyzerConfig in config.Analyzers)
            {
                IAnalyzer analyzer = analyzerResolver(analyzerConfig.Name);
                analyzer.Initialize(this, analyzerConfig.Args);

                analyzers.Add(analyzer);
                AddToReceiverLists(analyzer);
            }
        }

        /// <summary>
        /// Sets up the trading engine.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="tickInterval"></param>
        public async Task RunAsync()
        {
            if (config.RunMode == RunMode.Live || config.RunMode == RunMode.Paper)
            {
                await DataSource.StreamTicks(
                    config.Symbols.ToArray(),
                    config.Interval,
                    DateTime.UtcNow.GetNthPreviousTradingDay(Strategy.Lookback + 1),
                    null,
                    SendOnTickEvents);

                // setup live streaming
            }
            else if (config.RunMode == RunMode.BackTest)
            {
                await DataSource.StreamTicks(
                    config.Symbols.ToArray(),
                    config.Interval,
                    config.Start.Value,
                    config.End.Value,
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