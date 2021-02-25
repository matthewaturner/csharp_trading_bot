using Bot.Models;
using Bot.Exceptions;
using System;

namespace Bot.Indicators
{
    public class MovingAverageCrossover : IIndicator
    {
        private SimpleMovingAverage shortMA;
        private SimpleMovingAverage longMA;

        public MovingAverageCrossover(
            int shortLookback, 
            int longLookback, 
            Func<ITicks, double> transform)
        {
            if (shortLookback >= longLookback)
            {
                throw new ArgumentException("shortLookback must be <= longLookback.");
            }

            shortMA = new SimpleMovingAverage(shortLookback, transform);
            longMA = new SimpleMovingAverage(longLookback, transform);
            Lookback = longLookback;
        }

        public object Value
        {
            get
            {
                if (!Hydrated)
                {
                    throw new NotHydratedException();
                }
                return Helpers.CompareDoubles((double)shortMA.Value, (double)longMA.Value);
            }
        }

        public bool Hydrated
        {
            get { return shortMA.Hydrated && longMA.Hydrated; }
        }

        public int Lookback { get; private set; }

        public void OnTick(ITicks ticks)
        {
            shortMA.OnTick(ticks);
            longMA.OnTick(ticks);
        }
    }
}
