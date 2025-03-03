
using Bot.Models;
using Bot.Indicators.Interfaces;
using System;
using Bot.Helpers;

namespace Bot.Indicators
{
    public class MovingStandardDeviation : IndicatorBase<double>, ISimpleValueIndicator<double>
    {
        private readonly Func<MultiBar, double> transform;
        private double[] data;
        private int index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lookback"></param>
        /// <param name="transform"></param>
        public MovingStandardDeviation(int lookback, Func<MultiBar, double> transform)
            : base(lookback)
        {
            if (lookback < 1)
            {
                throw new ArgumentException("Lookback must be >= 1");
            }

            this.transform = transform;
            data = new double[lookback];
        }

        public override string Name => $"MSTD-{Lookback}";

        public double Value { get; private set; }

        /// <summary>
        /// Calculate new values.
        /// </summary>
        /// <param name="bars"></param>
        public override void OnBar(MultiBar bars)
        {
            data[index] = transform(bars);
            index = (index + 1) % this.Lookback;

            if (!this.IsHydrated && index == 0)
            {
                this.IsHydrated = true;
            }

            Value = MathHelpers.StdDev(data);
        }

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} = {Value}";
        }
    }
}
