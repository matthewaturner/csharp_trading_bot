using Bot.DataCollection;
using Bot.DataStorage;
using Bot.Engine;
using Bot.Strategies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Bot.Program;

namespace Bot.Trading
{
    public class TradingEngine : ITradingEngine
    {
        public readonly DataSourceResolver dataSourceResolver;
        public readonly BrokerResolver brokerResolver;
        public readonly StrategyResolver strategyResolver;
        public readonly ITickStorage tickStorage;
        public readonly Ticks ticks;

        public TradingEngine(
            DataSourceResolver dataSourceResolver,
            BrokerResolver brokerResolver,
            StrategyResolver strategyResolver,
            ITickStorage tickStorage,
            Ticks ticks)
        {
            this.dataSourceResolver = dataSourceResolver;
            this.brokerResolver = brokerResolver;
            this.strategyResolver = strategyResolver;
            this.tickStorage = tickStorage;
            this.ticks = ticks;
        }

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
            string[] strategyArgs = new string[] { "16", "64" };

            // resolve objects
            IDataSource dataSource = dataSourceResolver(dataSourceName);
            IBroker broker = brokerResolver(brokerName);
            IStrategy strategy = strategyResolver(strategyName);

            // initialize stuff
            ticks.Initialize(new string[] { symbol });
            dataSource.Initialize(dataSourceArgs);
            broker.Initialize(brokerArgs);
            strategy.Initialize(strategyArgs);

            IList<Tick> tickData = await tickStorage.GetTicksAsync(
                dataSource,
                symbol,
                TickInterval.Day,
                start,
                end);

            foreach (Tick tick in tickData)
            {
                var tickDictionary = new Dictionary<string, Tick>();
                tickDictionary.Add(symbol, tick);
                ticks.Update(tickDictionary);

                Console.WriteLine($"Tick: {tick}");
                Console.WriteLine($"Indicators Hydrated: {strategy.Hydrated}");

                broker.OnTick();
                strategy.OnTick();

                Console.WriteLine(broker.Portfolio);
                Console.WriteLine($"Portfolio Value:{broker.Portfolio.CurrentValue(ticks, (t) => t.AdjClose)}");
                Console.WriteLine("----------");
            }
        }
    }
}
