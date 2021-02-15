using Bot.Brokerages;
using Bot.Exceptions;
using System;

namespace Bot.Indicators
{
    public class MovingAverageCrossover : IIndicator<int>
    {
        private SimpleMovingAverage shortMA;
        private SimpleMovingAverage longMA;

        public MovingAverageCrossover(
            int shortLookback, 
            int longLookback, 
            Func<Tick, double> transform)
        {
            if (shortLookback >= longLookback)
            {
                throw new ArgumentException("shortLookback must be <= longLookback.");
            }

            shortMA = new SimpleMovingAverage(shortLookback, transform);
            longMA = new SimpleMovingAverage(longLookback, transform);
            Lookback = longLookback;
        }

        public int Value
        {
            get
            {
                if (!Hydrated)
                {
                    throw new IndicatorNotHydratedException();

                }
                return Helpers.CompareDoubles(shortMA.Value, longMA.Value);
            }
        }

        public int Lookback { get; private set; }

        public bool Hydrated
        {
            get { return shortMA.Hydrated && longMA.Hydrated; }
        }

        public void OnTick(Tick tick)
        {
            shortMA.OnTick(tick);
            longMA.OnTick(tick);
        }
    }
}
