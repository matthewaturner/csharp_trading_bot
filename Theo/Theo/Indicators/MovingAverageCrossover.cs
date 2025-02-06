using Theo.Models;
using Theo.Indicators.Interfaces;
using Theo.Exceptions;
using System;

namespace Theo.Indicators
{
    public class MovingAverageCrossover : IndicatorBase<PositionType>, IMovingAverageCrossover
    {
        private SimpleMovingAverage shortMA;
        private SimpleMovingAverage longMA;
        private PositionType position;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="shortLookback"></param>
        /// <param name="longLookback"></param>
        /// <param name="transform"></param>
        public MovingAverageCrossover(
            int shortLookback,
            int longLookback,
            Func<IMultiTick, double> transform)
            : base(longLookback)
        {
            if (shortLookback >= longLookback)
            {
                throw new ArgumentException("shortLookback must be <= longLookback.");
            }

            shortMA = new SimpleMovingAverage(shortLookback, transform);
            longMA = new SimpleMovingAverage(longLookback, transform);
            Lookback = longLookback;
            position = PositionType.Neutral;
        }

        public override string Name => $"MAC-{Lookback}";

        public PositionType Value => position;

        public double ShortMa => shortMA.Value;

        public double LongMa => longMA.Value;

        /// <summary>
        /// Calculate new values.
        /// </summary>
        /// <param name="ticks"></param>
        public override void OnTick(IMultiTick ticks)
        {
            shortMA.OnTick(ticks);
            longMA.OnTick(ticks);
            IsHydrated = shortMA.IsHydrated && longMA.IsHydrated;

            if (IsHydrated)
            {
                position = (PositionType)Helpers.CompareDoubles(shortMA.Value, longMA.Value);
            }
        }

        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }
}
