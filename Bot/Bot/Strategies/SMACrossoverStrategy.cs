using Bot.Indicators;
using Bot.Engine;
using System;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public class SMACrossoverStrategy : StrategyBase, IStrategy
    {
        private ITicks ticks;
        private IList<IIndicator> indicators;
        private IIndicator mac;

        public SMACrossoverStrategy(ITicks ticks)
        {
            this.ticks = ticks;
        }

        public void Initialize(string[] args)
        {
            int shortLookback = int.Parse(args[0]);
            int longLookback = int.Parse(args[1]);

            mac = new MovingAverageCrossover(
                shortLookback, 
                longLookback, 
                (Tick t) => t.AdjClose);
            indicators = new List<IIndicator> { mac };
        }

        public override IList<IIndicator> Indicators => indicators;

        /// <summary>
        /// Logic here!
        /// </summary>
        /// <param name="tick"></param>
        public override void OnTick()
        {
            Console.WriteLine("Got a tick!");
        }
    }
}
