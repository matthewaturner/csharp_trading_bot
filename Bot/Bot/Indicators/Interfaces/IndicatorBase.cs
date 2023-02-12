
using Bot.Models;
using System.Collections.Generic;

namespace Bot.Indicators.Interfaces
{
    public abstract class IndicatorBase<T> : IIndicator
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public IndicatorBase(
            int lookback,
            bool isHydrated = false)
        {
            this.Lookback = lookback;
            this.IsHydrated = isHydrated;
        }

        /// <summary>
        /// Must be overridden in base class.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Cannot be overridden.
        /// </summary>
        public int Lookback { get; protected set; }

        /// <summary>
        /// Can be overridden.
        /// </summary>
        public virtual bool IsHydrated { get; protected set; }

        /// <summary>
        /// Calculate new values.
        /// </summary>
        /// <param name="ticks"></param>
        public abstract void OnTick(IMultiTick ticks);
    }
}
