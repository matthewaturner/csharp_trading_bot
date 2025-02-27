﻿
using Bot.Models;
using Bot.Indicators.Interfaces;
using System;

namespace Bot.Indicators
{
    public class SimpleMovingAverage : IndicatorBase<decimal>, ISimpleValueIndicator<decimal>
    {
        private readonly Func<MultiBar, decimal> transform;
        private decimal[] data;
        private decimal average;
        private int index;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lookback"></param>
        /// <param name="transform"></param>
        public SimpleMovingAverage(int lookback, Func<MultiBar, decimal> transform)
            : base(lookback)
        {
            if (lookback < 1)
            {
                throw new ArgumentException("Lookback must be >= 1.");
            }

            this.transform = transform;
            data = new decimal[lookback];
            index = 0;
            average = 0;
        }

        public override string Name => $"SMA-{Lookback}";

        public decimal Value => average;

        /// <summary>
        /// Calculates new values.
        /// </summary>
        /// <param name="bars"></param>
        public override void OnBar(MultiBar bars)
        {
            average = average - (data[index] / Lookback);
            data[index] = transform(bars);
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
