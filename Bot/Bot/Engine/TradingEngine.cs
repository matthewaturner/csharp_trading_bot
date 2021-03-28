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
        public ITicks Ticks => ticks;

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
        /// <param name="configFileName"></param>
        public void Initialize(EngineConfig config)
        {
            this.config = config;
            ClearReceiverLists();

            // setup path for output
            OutputPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                $"/{config.Strategy.Name}.{DateTimeOffset.Now.ToUnixTimeSeconds()}");
            Directory.CreateDirectory(OutputPath);

            // initialize stuff
            ticks = new Ticks(config.Symbols.ToArray());
            symbols = config.Symbols;

            // data source
            dataSource = dataSourceResolver(config.DataSource.Name);
            dataSource.Initialize(this, config.DataSource.Args);
            AddToReceiverLists(dataSource);

            // broker - needs to come before strategy
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

        /// <summary>
        /// Sets up the trading engine.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="tickInterval"></param>
        public async Task RunAsync()
        {
            await StreamTicks(
                config.Symbols.ToArray(),
                config.Interval,
                config.Start,
                config.End,
                SendOnTickEvents);

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

        /// <summary>
        /// Call get ticks for each symbol and unify into one multi-tick stream of events.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="symbols"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="onTickCallback"></param>
        private async Task StreamTicks(
            string[] symbols,
            TickInterval interval,
            DateTime start,
            DateTime end,
            Action<Tick[]> onTickCallback)
        {
            IList<Tick>[] allTicks = new List<Tick>[symbols.Length];
            int tickCount = 0;

            for (int i = 0; i < symbols.Length; i++)
            {
                allTicks[i] = await dataSource.GetTicksAsync(symbols[i], interval, start, end);

                if (tickCount == 0)
                {
                    tickCount = allTicks[i].Count;
                }
                else if (allTicks[i].Count != tickCount)
                {
                    throw new BadDataException("Received varying number of ticks from each symbol.");
                }
            }

            Tick[] tickArray = new Tick[symbols.Length];
            Ticks currentTicks = new Ticks(symbols);
            IList<IEnumerator<Tick>> enumerators = new List<IEnumerator<Tick>>();

            for (int i = 0; i < symbols.Length; i++)
            {
                enumerators.Add(allTicks[i].GetEnumerator());
            }

            while (enumerators.All(e => e.MoveNext()))
            {
                for (int i = 0; i < enumerators.Count; i++)
                {
                    tickArray[i] = enumerators[i].Current;
                }

                onTickCallback(tickArray);
            }
        }
    }
}