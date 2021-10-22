
using Bot.Models;
using System;

namespace Bot.Indicators
{
    public class SimpleMovingAverage : IndicatorBase
    {
        private readonly Func<IMultiBar, double> transform;
        private double[] data;
        private double average;
        private int index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lookback"></param>
        /// <param name="transform"></param>
        public SimpleMovingAverage(int lookback, Func<IMultiBar, double> transform)
            : base()
        {
            if (lookback < 1)
            {
                throw new ArgumentException("Lookback must be >= 1.");
            }

            this.transform = transform;
            data = new double[lookback];
            index = 0;
            average = 0;

            Hydrated = false;
            Lookback = lookback;
        }

        /// <summary>
        /// Calculates new values.
        /// </summary>
        /// <param name="ticks"></param>
        public override void OnTick(IMultiBar ticks)
        {
            average = average - (data[index] / Lookback);
            data[index] = transform(ticks);
            average = average + (data[index] / Lookback);

            index = (index + 1) % Lookback;

            if (!Hydrated && index == 0)
            {
                Hydrated = true;
            }

            Values["default"] = average;
            Values["ma"] = average;
        }
    }
}
