
using Bot.Engine;
using Bot.Engine.Events;
using Bot.Indicators;
using Bot.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Strategies
{
    public abstract class StrategyBase : IStrategy, ITickReceiver
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

        /// <summary>
        /// List of indicators used to calculate lookback and hydrated values.
        /// </summary>
        public abstract IList<IIndicator> Indicators { get; }

        /// <summary>
        /// Get csv headers.
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetCsvHeaders()
        {
            return new string[] { };
        }

        /// <summary>
        /// Get csv values.
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetCsvValues()
        {
            return new string[] { };
        }

        /// <summary>
        /// Overridden in strategy.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public abstract void Initialize(ITradingEngine engine, string[] args);

        /// <summary>
        /// OnTick method which call the strategy on tick when indicators are hydrated.
        /// </summary>
        /// <param name="ticks"></param>
        public void OnTick(IMultiTick ticks)
        {
            foreach (IIndicator ind in Indicators)
            {
                ind.OnTick(ticks);
            }

            if (Indicators.All(ind => ind.Hydrated))
            {
                StrategyOnTick(ticks);
            }
        }

        /// <summary>
        /// Strategy logic goes here.
        /// </summary>
        /// <param name="ticks"></param>
        public abstract void StrategyOnTick(IMultiTick ticks);
    }
}
