
using Bot.Brokers;
using Bot.Exceptions;
using System;

namespace Bot.Indicators
{
    public class SimpleMovingAverage : IIndicator
    {
        private readonly Func<ITicks, double> transform;
        private double[] data;
        private double average;
        private int index;

        public SimpleMovingAverage(int lookback, Func<ITicks, double> transform)
        {
            this.transform = transform;
            data = new double[lookback];
            index = 0;
            average = 0;

            Hydrated = false;
            Lookback = lookback;
        }

        public object Value
        {
            get
            {
                if (!Hydrated)
                {
                    throw new NotHydratedException();
                }
                return average;
            }
        }

        public bool Hydrated { get; private set; }

        public int Lookback { get; private set; }

        public void OnTick(ITicks ticks)
        {
            average = average - (data[index] / Lookback);
            data[index] = transform(ticks);
            average = average + (data[index] / Lookback);

            index = (index + 1) % Lookback;

            if (!Hydrated && index == 0)
            {
                Hydrated = true;
            }
        }
    }
}
