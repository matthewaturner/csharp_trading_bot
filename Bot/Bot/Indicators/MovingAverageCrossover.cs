using Bot.Models;
using Bot.Exceptions;
using System;

namespace Bot.Indicators
{
    public class MovingAverageCrossover : IndicatorBase
    {
        private SimpleMovingAverage shortMA;
        private SimpleMovingAverage longMA;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shortLookback"></param>
        /// <param name="longLookback"></param>
        /// <param name="transform"></param>
        public MovingAverageCrossover(
            int shortLookback, 
            int longLookback, 
            Func<IMultiBar, double> transform)
            : base()
        {
            if (shortLookback >= longLookback)
            {
                throw new ArgumentException("shortLookback must be <= longLookback.");
            }

            shortMA = new SimpleMovingAverage(shortLookback, transform);
            longMA = new SimpleMovingAverage(longLookback, transform);
            Lookback = longLookback;
        }

        /// <summary>
        /// Calculate new values.
        /// </summary>
        /// <param name="ticks"></param>
        public override void OnTick(IMultiBar ticks)
        {
            shortMA.OnTick(ticks);
            longMA.OnTick(ticks);

            if (!Hydrated && shortMA.Hydrated && longMA.Hydrated)
            {
                Hydrated = true;
            }

            Values["default"] = Helpers.CompareDoubles(shortMA.Value, longMA.Value);
        }
    }
}
