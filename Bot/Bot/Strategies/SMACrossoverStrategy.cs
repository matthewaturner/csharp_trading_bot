using Bot.Indicators;
using Bot.Models;
using System;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public class SMACrossoverStrategy : StrategyBase
    {
        private IList<IIndicator> indicators;
        private IIndicator mac;
        private readonly int longLookback;

        public SMACrossoverStrategy(int shortLookback, int longLookback)
        {
            this.longLookback = longLookback;
            mac = new MovingAverageCrossover(
                shortLookback, 
                longLookback, 
                (Tick t) => t.AdjClose);
            indicators = new List<IIndicator> { mac };
        }

        public override int Lookback => longLookback;

        public override bool Hydrated => mac.Hydrated;

        public override IList<IIndicator> Indicators => indicators;

        /// <summary>
        /// Logic here!
        /// </summary>
        /// <param name="tick"></param>
        public override void OnTick(Tick tick)
        {
            Console.WriteLine("Got a tick!");
        }
    }
}
