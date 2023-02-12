
using Bot.Models;
using Bot.Indicators.Interfaces;
using System;

namespace Bot.Indicators
{
    public class SimpleMovingAverage : IndicatorBase<double>, ISimpleValueIndicator<double>
    {
        private readonly Func<IMultiTick, double> transform;
        private double[] data;
        private double average;
        private int index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lookback"></param>
        /// <param name="transform"></param>
        public SimpleMovingAverage(int lookback, Func<IMultiTick, double> transform)
            : base(lookback)
        {
            if (lookback < 1)
            {
                throw new ArgumentException("Lookback must be >= 1.");
            }

            this.transform = transform;
            data = new double[lookback];
            index = 0;
            average = 0;
        }

        public override string Name => $"SMA-{Lookback}";

        public double Value => average;

        /// <summary>
        /// Calculates new values.
        /// </summary>
        /// <param name="ticks"></param>
        public override void OnTick(IMultiTick ticks)
        {
            average = average - (data[index] / Lookback);
            data[index] = transform(ticks);
            average = average + (data[index] / Lookback);

            index = (index + 1) % Lookback;

            if (!IsHydrated && index == 0)
            {
                IsHydrated = true;
            }
        }

        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }
}
