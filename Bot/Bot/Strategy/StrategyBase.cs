
using Bot.Engine;
using Bot.Indicators;
using Bot.Models;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Strategies
{
    public abstract class StrategyBase : IStrategy
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public StrategyBase()
        {
            Indicators = new List<IIndicator>();
        }

        public int Lookback => Indicators.Max(ind => ind.Lookback);

        public bool IsHydrated => Indicators.All(ind => ind.IsHydrated);

        public IList<IIndicator> Indicators { get; set; }

        protected ITradingEngine Engine { get; set; }

        /// <summary>
        /// Setup the strategy.
        /// </summary>
        /// <param name="engine"></param>
        public void Initialize(ITradingEngine engine)
        { 
            Engine = engine;
        }

        /// <summary>
        /// OnTick method which call the strategy on tick when indicators are hydrated.
        /// </summary>
        /// <param name="ticks"></param>
        public void BaseOnTick(IMultiTick ticks)
        {
            foreach (IIndicator ind in Indicators)
            {
                ind.OnTick(ticks);
            }

            Engine.Logger.LogVerbose($"Got Ticks: {ticks}");
            Engine.Logger.LogVerbose($"Current Indicators: {string.Join(", ", Indicators)}");

            if (Indicators.All(ind => ind.IsHydrated))
            {
                OnTick(ticks);
            }
            else
            {
                Engine.Logger.LogVerbose($"Indicators not hydrated. Skipping strategy logic.");
            }
        }

        /// <summary>
        /// Strategy logic goes here.
        /// </summary>
        /// <param name="ticks"></param>
        public abstract void OnTick(IMultiTick ticks);
    }
}
