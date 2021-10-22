using Bot.Analyzers;
using Bot.Brokers;
using Bot.Configuration;
using Bot.Data;
using Bot.Models;
using Bot.Strategies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Engine
{
    public interface ITradingEngine
    {
        /// <summary>
        /// Sets up the trading engine.
        /// </summary>
        /// <param name="configFileName"></param>
        public void Initialize(EngineConfig config);

        /// <summary>
        /// Gets the symbols we are running for.
        /// </summary>
        public IList<string> Symbols { get; }

        /// <summary>
        /// Get current ticks.
        /// </summary>
        public IMultiBar Ticks { get; }

        /// <summary>
        /// Gets current data source.
        /// </summary>
        public IDataSource DataSource { get; }

        /// <summary>
        /// Get current broker.
        /// </summary>
        public IBroker Broker { get; }

        /// <summary>
        /// Gets current strategy.
        /// </summary>
        public IStrategy Strategy { get; }

        /// <summary>
        /// Gets all analyzers.
        /// </summary>
        public IList<IAnalyzer> Analyzers { get; }

        /// <summary>
        /// Gets the path for output data.
        /// </summary>
        public string OutputPath { get; }

        /// <summary>
        /// Runs the engine.
        /// </summary>
        /// <param name="engineConfig"></param>
        /// <returns></returns>
        public Task RunAsync();

        /// <summary>
        /// Sends log events to listeners.
        /// </summary>
        /// <param name="log"></param>
        public void Log(string log);
    }
}
