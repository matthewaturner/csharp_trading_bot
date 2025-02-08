
using Theo.Engine;
using Theo.Indicators;
using Theo.Models;
using System.Collections.Generic;
using System.Linq;

namespace Theo.Strategies
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
        /// OnBar method which call the strategy on bar when indicators are hydrated.
        /// </summary>
        /// <param name="bars"></param>
        public void BaseOnBar(MultiBar bars)
        {
            foreach (IIndicator ind in Indicators)
            {
                ind.OnBar(bars);
            }

            Engine.Logger.LogVerbose($"Got Bars: {bars}");
            Engine.Logger.LogVerbose($"Current Indicators: {string.Join(", ", Indicators)}");

            if (Indicators.All(ind => ind.IsHydrated))
            {
                OnBar(bars);
            }
            else
            {
                Engine.Logger.LogVerbose($"Indicators not hydrated. Skipping strategy logic.");
            }
        }

        /// <summary>
        /// Strategy logic goes here.
        /// </summary>
        /// <param name="bars"></param>
        public abstract void OnBar(MultiBar bars);
    }
}
