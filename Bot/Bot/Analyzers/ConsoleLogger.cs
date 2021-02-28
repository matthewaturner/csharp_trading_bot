
using Bot.Models;
using Bot.Engine;
using System;
using Bot.Engine.Events;

namespace Bot.Analyzers
{
    public class ConsoleLogger : IAnalyzer, ITickReceiver, ITerminateReceiver
    {
        private ITradingEngine engine;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public void Initialize(ITradingEngine engine, string[] args)
        {
            this.engine = engine;
        }

        /// <summary>
        /// When a new tick comes in.
        /// </summary>
        /// <param name="ticks"></param>
        public void OnTick(ITicks ticks)
        {
            Console.WriteLine($"Tick: {ticks}");
        }

        /// <summary>
        /// When everything is done.
        /// </summary>
        public void OnTerminate()
        {
            string output = "Summary Data:\n";
            foreach (IAnalyzer analyzer in engine.Analyzers)
            {
                if (analyzer != this)
                {
                    output += "\t" + analyzer.ToString() + "\n";
                }
            }
            Console.WriteLine(output);
        }
    }
}
