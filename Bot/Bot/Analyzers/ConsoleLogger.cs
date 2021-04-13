
using Bot.Models;
using Bot.Engine;
using System;
using Bot.Engine.Events;
using System.Linq;

namespace Bot.Analyzers
{
    public class ConsoleLogger : IAnalyzer, ITerminateReceiver
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

            string orderHistory = string.Join(',', engine.Broker.GetAllOrders().Select(o => o.ToString() + "\n"));
            Console.WriteLine($"Order History: {orderHistory}");

            Console.WriteLine(output);
        }
    }
}
