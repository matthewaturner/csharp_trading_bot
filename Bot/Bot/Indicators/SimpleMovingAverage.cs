
using Bot.Models;
using Bot.Exceptions;
using System;

namespace Bot.Indicators
{
    public class SimpleMovingAverage : IIndicator<double>
    {
        private readonly Func<Tick, double> transform;
        private double[] data;
        private double average;
        private int index;

        public SimpleMovingAverage(int lookback, Func<Tick, double> transform)
        {
            this.transform = transform;
            this.lookback = lookback;
            data = new double[lookback];
            index = 0;
            average = 0;

            Hydrated = false;
            Lookback = lookback;
        }

        public double Value
        {
            get
            {
                if (!IsHydrated())
                {
                    throw new IndicatorNotHydratedException();
                }
                return average;
            }
        }

        public bool Hydrated { get; private set; }

        public int Lookback { get; private set; }

        public void OnTick(Tick tick)
        {
            average = average - (data[index] / Lookback);
            data[index] = transform(tick);
            average = average + (data[index] / Lookback);

            index = (index + 1) % Lookback;

            if (!Hydrated && index == 0)
            {
                Hydrated = true;
            }
        }

        public bool IsHydrated()
        {
            return isHydrated;
        }
    }
}
