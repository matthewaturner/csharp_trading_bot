
using Bot.Indicators;
using Bot.Models;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public abstract class StrategyBase : IStrategy
    {
        /// <summary>
        /// Base on tick method, simply calls regular on tick if it is hydrated.
        /// </summary>
        /// <param name="ticks"></param>
        public void OnTickBase(Tick tick)
        {
            for (int i=0; i<Indicators.Count; i++)
            {
                Indicators[i].OnTick(tick);
            }

            if (Hydrated)
            {
                OnTick();
            }
        }

        /// <summary>
        /// Returns the max lookback for any of the indicators.
        /// </summary>
        public virtual int Lookback { get; }

        /// <summary>
        /// Returns whether all indicators are hydrated for the strategy.
        /// </summary>
        public virtual bool Hydrated { get; }

        /// <summary>
        /// OnTick which must be overrided in the child class.
        /// </summary>
        /// <param name="tick"></param>
        public abstract void OnTick();

        /// <summary>
        /// Gets a list of indicators, makes hydration easier.
        /// </summary>
        public abstract IList<IIndicator> Indicators { get; }

    }
}
