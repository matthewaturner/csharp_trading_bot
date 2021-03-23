using Bot.Exceptions;
using Bot.Models;
using System;

namespace Bot.Indicators
{
    public class MovingStandardDeviation : IIndicator
    {
        private readonly Func<ITicks, double> transform;
        private double[] data;
        private int index;
        private double stdDev;

        public MovingStandardDeviation(int lookback, Func<ITicks, double> transform)
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

        public int Lookback { get; private set; }

        public bool Hydrated { get; private set; }

        public object Value
        {
            get
            {
                if (!Hydrated)
                {
                    throw new NotHydratedException();
                }
                return stdDev;
            }
        }

        public void OnTick(ITicks ticks)
        {
            data[index] = transform(ticks);
            index = (index + 1) % Lookback;

            stdDev = Helpers.StandardDeviation(data);

            if (!Hydrated && index == 0)
            {
                Hydrated = true;
            }
        }
    }
}
