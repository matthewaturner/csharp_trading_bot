
using Bot.Brokerages;
using System;

namespace Bot.Indicators
{
    public class SimpleMovingAverage : IIndicator<double?>
    {
        private readonly Func<Tick, double> transform;
        private double[] data;
        private double average;
        private int index;
        private int lookback;
        private bool isHydrated;

        public SimpleMovingAverage(int lookback, Func<Tick, double> transform)
        {
            this.transform = transform;
            this.lookback = lookback;
            data = new double[lookback];
            index = 0;
            average = 0;
            isHydrated = false;
        }

        public double? Value
        {
            get
            {
                return IsHydrated() ? (double?)average : null;
            }
        }

        public void OnTick(Tick tick)
        {
            average = average - (data[index] / lookback);
            data[index] = this.transform(tick);
            average = average + (data[index] / lookback);

            index = (index + 1) % lookback;

            if (!isHydrated && index == 0)
            {
                isHydrated = true;
            }
        }

        public bool IsHydrated()
        {
            return isHydrated;
        }
    }
}
