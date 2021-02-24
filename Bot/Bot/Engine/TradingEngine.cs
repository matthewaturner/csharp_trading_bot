using Bot.DataCollection;
using Bot.DataStorage;
using Bot.Brokers;
using Bot.Strategies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Bot.Program;
using Bot.Analyzers;

namespace Bot.Engine
{
    public class TradingEngine : ITradingEngine
    {
        public readonly DataSourceResolver dataSourceResolver;
        public readonly BrokerResolver brokerResolver;
        public readonly StrategyResolver strategyResolver;
        public readonly AnalyzerResolver analyzerResolver;

        public Ticks ticks;
        public ITickStorage tickStorage;
        public IList<ITickReceiver> tickReceivers;

        private IBroker broker;
        private IDataSource dataSource;
        private IStrategy strategy;
        private IList<IAnalyzer> analyzers;

        public TradingEngine(
            DataSourceResolver dataSourceResolver,
            BrokerResolver brokerResolver,
            StrategyResolver strategyResolver,
            AnalyzerResolver analyzerResolver,
            ITickStorage tickStorage)
        {
            this.dataSourceResolver = dataSourceResolver;
            this.brokerResolver = brokerResolver;
            this.strategyResolver = strategyResolver;
            this.analyzerResolver = analyzerResolver;
            this.tickStorage = tickStorage;
            tickReceivers = new List<ITickReceiver>();
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
        /// Configure a strategy and run it.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="tickInterval"></param>
        public async Task RunAsync()
        {
            // config
            string symbol = "MSFT";
            TickInterval interval = TickInterval.Day;
            DateTime start = new DateTime(2010, 1, 1);
            DateTime end = new DateTime(2020, 1, 1);

            string dataSourceName = nameof(YahooDataSource);
            string[] dataSourceArgs = new string[] { };

            string brokerName = nameof(BackTestingBroker);
            string[] brokerArgs = new string[] { "1000" };

            string strategyName = nameof(SMACrossoverStrategy);
            string[] strategyArgs = new string[] { symbol, "16", "64", "true" };

            string[] analyzerList = new string[] { "ConsoleLogger" };

            // initialize stuff
            ticks = new Ticks(new string[] { symbol });

            // data source
            IDataSource dataSource = dataSourceResolver(dataSourceName);
            dataSource.Initialize(this, dataSourceArgs);
            AddToReceiverLists(dataSource);

            // broker
            broker = brokerResolver(brokerName);
            broker.Initialize(this, brokerArgs);
            AddToReceiverLists(broker);

            // strategy
            strategy = strategyResolver(strategyName);
            strategy.Initialize(this, strategyArgs);
            AddToReceiverLists(strategy);

            // analyzers
            IList<IAnalyzer> analyzers = new List<IAnalyzer>();
            foreach (string name in analyzerList)
            {
                IAnalyzer analyzer = analyzerResolver(name);
                analyzers.Add(analyzerResolver(name));
                AddToReceiverLists(analyzer);
            }

            IList<Tick> tickData = await tickStorage.GetTicksAsync(
                dataSource,
                symbol,
                interval,
                start,
                end);

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
        }

        /// <summary>
        /// Adds the objects to the receiver lists for the different types of events.
        /// </summary>
        /// <param name="obj"></param>
        private void AddToReceiverLists(object obj)
        {
            if (obj is ITickReceiver receiver)
            {
                tickReceivers.Add(receiver);
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
    }
}