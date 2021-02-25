using Bot.Analyzers;
using Bot.Models;
using Bot.Strategies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Engine
{
    public interface ITradingEngine
    {
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

        /// <summary>
        /// Runs the trading engine.
        /// </summary>
        /// <returns></returns>
        public Task RunAsync(string configFile);
    }
}
