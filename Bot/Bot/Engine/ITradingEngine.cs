using Bot.Analyzers;
using Bot.Configuration;
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
        public void Initialize(string configFileName);

        /// <summary>
        /// Get current ticks.
        /// </summary>
        public ITicks Ticks { get; }

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

        public Task RunAsync(EngineConfig engineConfig);

        /// <summary>
        /// Sends log events to listeners.
        /// </summary>
        /// <param name="log"></param>
        public void Log(string log);
    }
}
