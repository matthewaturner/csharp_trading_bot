
using Bot.Indicators;
using Bot.Engine;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Strategies
{
    public abstract class StrategyBase
    {
        
        /// <summary>
        /// Returns the max lookback for any of the indicators.
        /// </summary>
        public int Lookback
        {
            get { return Indicators.Max(ind => ind.Lookback); }
        }

        /// <summary>
        /// Returns whether all indicators are hydrated for the strategy.
        /// </summary>
        public bool Hydrated
        {
            get { return Indicators.All(ind => ind.Hydrated); }
        }
    }
}
