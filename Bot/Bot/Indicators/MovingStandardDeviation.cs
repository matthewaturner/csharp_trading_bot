
using Bot.Models;
using System;

namespace Bot.Indicators
{
    public class MovingStandardDeviation : IndicatorBase
    {
        private readonly Func<IMultiTick, double> transform;
        private double[] data;
        private int index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lookback"></param>
        /// <param name="transform"></param>
        public MovingStandardDeviation(int lookback, Func<IMultiTick, double> transform)
            : base()
        {
            if (lookback < 1)
            {
                throw new ArgumentException("Lookback must be >= 1");
            }

            this.transform = transform;
            data = new double[lookback];

            Lookback = lookback;
            Hydrated = false;
        }

        /// <summary>
        /// Calculate new values.
        /// </summary>
        /// <param name="ticks"></param>
        public override void OnTick(IMultiTick ticks)
        {
            data[index] = transform(ticks);
            index = (index + 1) % Lookback;

            if (!Hydrated && index == 0)
            {
                Hydrated = true;
            }

            Values["default"] = Helpers.StandardDeviation(data);
            Values["stdDev"] = Values["default"];
        }
    }
}
