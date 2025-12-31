
using Bot.Analyzers;
using Bot.Brokers;
using Bot.DataSources;
using Bot.Models.Allocations;
using Bot.Models.Engine;
using Bot.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Engine;

public partial class TradingEngine
{
    /// <summary>
    /// Buddy class inside of trading engine.
    /// </summary>
    public class EngineBuilder
    {
        private RunConfig _runConfig;
        private MetaAllocation _metaAllocation = new MetaAllocation();
        private IStrategyAnalyzer _analyzer = new StrategyAnalyzer();
        private IDataSource _dataSource;
        private List<IStrategy> _strategies = new();

        // add a config
        public EngineBuilder WithConfig(RunConfig config)
        {
            _runConfig = config;
            return this;
        }

        // add a data source
        public EngineBuilder WithDataSource(IDataSource dataSource)
        {
            _dataSource = dataSource;
            return this;
        }

        // add a strategy and give it a portfolio weight
        public EngineBuilder WithStrategy(IStrategy strategy, double strategyWeight)
        {
            if (_strategies.Count > 1)
                throw new NotImplementedException("Only one strategy is supported at this time.");

            _strategies.Add(strategy);
            _metaAllocation.SetStrategyWeight(strategy.Id, strategyWeight);
            return this;
        }

        // add an execution engine with a broker
        public EngineBuilder WithExecutionEngine(IBroker broker, double rebalanceThreshold = 0.01)
        {
            _analyzer = new ExecutionEngine(broker, rebalanceThreshold);
            return this;
        }

        // use the default strategy analyzer (for theoretical performance)
        public EngineBuilder WithStrategyAnalyzer()
        {
            _analyzer = new StrategyAnalyzer();
            return this;
        }

        /// <summary>
        /// Build the engine.
        /// </summary>
        public TradingEngine Build()
        {
            return new TradingEngine()
            {
                RunConfig = _runConfig,
                MetaAllocation = _metaAllocation,
                Analyzer = _analyzer,
                DataSource = _dataSource,
                Strategy = _strategies.First() // todo
            };
        }
    }

}
