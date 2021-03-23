
using Bot.Exceptions;
using Bot.Models;
using System;

namespace Bot.Indicators
{
    public class BollingerBand : IIndicator
    {
        enum Position
        {
            Short = -1,
            Neutral = 0,
            Long = 1
        };

        SimpleMovingAverage sma;
        MovingStandardDeviation stdDev;
        Func<ITicks, double> transform;
        double zScore, entryZScore, exitZScore;
        Position position;

        public BollingerBand(
            int lookback, 
            double entryZScore, 
            double exitZScore, 
            Func<ITicks, double> transform)
        {
            if (lookback < 1)
            {
                throw new ArgumentException("Lookback must be >= 1.");
            }
            if (entryZScore < 0)
            {
                throw new ArgumentException("EntryZScore must be > 0.");
            }
            if (exitZScore < 0)
            {
                throw new ArgumentException("ExitZScore must be > 0.");
            }

            sma = new SimpleMovingAverage(lookback, transform);
            stdDev = new MovingStandardDeviation(lookback, transform);

            this.transform = transform;
            this.entryZScore = entryZScore;
            this.exitZScore = exitZScore;
            Lookback = lookback;
            zScore = double.NaN;
            position = Position.Neutral;
        }

        public int Lookback { get; private set; }

        public bool Hydrated { get { return sma.Hydrated && stdDev.Hydrated; } }

        public object Value
        {
            get
            {
                if (!Hydrated)
                {
                    throw new NotHydratedException();
                }
                return position;
            }
        }

        public void OnTick(ITicks ticks)
        {
            sma.OnTick(ticks);
            stdDev.OnTick(ticks);

            if (Hydrated)
            {
                zScore = (transform(ticks) - (double)sma.Value) / (double)stdDev.Value;

                // entry logic
                if (Math.Abs(zScore) > entryZScore)
                {
                    position = (Position)(-Math.Sign(zScore));
                }

                // exit logic
                if (position == Position.Long && zScore >= -exitZScore
                    || position == Position.Short && zScore <= exitZScore)
                {
                    position = Position.Neutral;
                }
            }

        }
    }
}
