
using Bot.Models;
using System;

namespace Bot.Indicators
{
    public class BollingerBand : IndicatorBase
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

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="lookback"></param>
        /// <param name="entryZScore"></param>
        /// <param name="exitZScore"></param>
        /// <param name="transform"></param>
        public BollingerBand(
            int lookback, 
            double entryZScore, 
            double exitZScore, 
            Func<ITicks, double> transform)
            : base()
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
                throw new ArgumentException("ExitZScore must be >= 0.");
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

        /// <summary>
        /// Recalculate values.
        /// </summary>
        /// <param name="ticks"></param>
        public override void OnTick(ITicks ticks)
        {
            sma.OnTick(ticks);
            stdDev.OnTick(ticks);

            if (!Hydrated && sma.Hydrated && stdDev.Hydrated)
            {
                Hydrated = true;
            }

            zScore = (transform(ticks) - sma.Value) / stdDev.Value;

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

            Values["default"] = (double)position;
            Values["position"] = (double)position;
            Values["upperBand"] = sma.Value + (entryZScore * stdDev.Value);
            Values["lowerBand"] = sma.Value - (entryZScore * stdDev.Value);
            Values["midBand"] = sma.Value;
        }
    }
}
