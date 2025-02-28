
using Bot.Models;
using Bot.Indicators.Interfaces;
using System;
using Bot.Helpers;

namespace Bot.Indicators
{
    public class MovingStandardDeviation : IndicatorBase<decimal>, ISimpleValueIndicator<decimal>
    {
        private readonly Func<MultiBar, decimal> transform;
        private decimal[] data;
        private int index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lookback"></param>
        /// <param name="transform"></param>
        public MovingStandardDeviation(int lookback, Func<MultiBar, decimal> transform)
            : base(lookback)
        {
            if (lookback < 1)
            {
                throw new ArgumentException("Lookback must be >= 1");
            }

            this.transform = transform;
            data = new decimal[lookback];
        }

        public override string Name => $"MSTD-{Lookback}";

        public decimal Value { get; private set; }

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

            Value = MathHelpers.StandardDeviation(data);
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
