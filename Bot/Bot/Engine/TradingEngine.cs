using Bot.Analyzers;
using Bot.Configuration;
using Bot.DataCollection;
using Bot.DataStorage;
using Bot.Engine.Events;
using Bot.Exceptions;
using Bot.Models;
using Bot.Strategies;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

using static Bot.Program;

namespace Bot.Engine
{
    public class TradingEngine : ITradingEngine
    {
        private readonly DataSourceResolver dataSourceResolver;
        private readonly BrokerResolver brokerResolver;
        private readonly StrategyResolver strategyResolver;
        private readonly AnalyzerResolver analyzerResolver;

        private EngineConfig config;
        private Ticks ticks;

        private IList<ITickReceiver> tickReceivers;
        private IList<ITerminateReceiver> terminateReceivers;
        private IList<ILogReceiver> logReceivers;

        private IBroker broker;
        private IDataSource dataSource;
        private IStrategy strategy;
        private IList<IAnalyzer> analyzers;

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
        /// Current ticks.
        /// </summary>
        public ITicks Ticks => ticks;

        /// <summary>
        /// Current broker.
        /// </summary>
        public IBroker Broker => broker;

        /// <summary>
        /// Current strategy.
        /// </summary>
        public IStrategy Strategy => strategy;

        /// <summary>
        /// Current analyzers.
        /// </summary>
        public IList<IAnalyzer> Analyzers => analyzers;

        /// <summary>
        /// Sets up the trading engine.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="tickInterval"></param>
        public async Task RunAsync(EngineConfig config)
        {

            // initialize stuff
            ticks = new Ticks(config.Symbols.ToArray());

            // data source
            IDataSource dataSource = dataSourceResolver(config.DataSource.Name);
            dataSource.Initialize(this, config.DataSource.Args);
            AddToReceiverLists(dataSource);

            // broker
            broker = brokerResolver(config.Broker.Name);
            broker.Initialize(this, config.Broker.Args);
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

            IList<Tick> tickData = await dataSource.GetTicksAsync(
                config.Symbols[0],
                config.Interval,
                config.Start,
                config.End,
                SendOnTickEvents);

            foreach (Tick tick in tickData)
            {
                ticks.Update(new Tick[] { tick });

                Console.WriteLine($"Tick: {tick}");
                Console.WriteLine($"Indicators Hydrated: {strategy.Hydrated}");

                SendOnTickEvents(ticks.ToArray());

                Console.WriteLine(broker.Portfolio);
                Console.WriteLine($"Portfolio Value:{broker.Portfolio.CurrentValue(ticks, (t) => t.AdjClose)}");
                Console.WriteLine("----------");
            }

            SendTerminateEvents();
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
        /// Sends on tick events to all interested parties.
        /// </summary>
        /// <param name="ticks"></param>
        private void SendOnTickEvents(Tick[] newTicks)
        {
            ticks.Update(newTicks);
            foreach (ITickReceiver receiver in tickReceivers)
            {
                receiver.OnTick(ticks);
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
        public void Log(string log)
        {
            foreach (ILogReceiver receiver in logReceivers)
            {
                receiver.OnLog(log);
            }
        }
    }
}