
using Bot.Brokers;
using Bot.Engine;
using System;

namespace Bot.Analyzers
{
    public class ConsoleLogger : IAnalyzer, ITickReceiver
    {
        public void Initialize(ITradingEngine engine, string[] args)
        { }

        public void OnTick(ITicks ticks)
        {
            Console.WriteLine($"Tick: {ticks}");
        }
    }
}
