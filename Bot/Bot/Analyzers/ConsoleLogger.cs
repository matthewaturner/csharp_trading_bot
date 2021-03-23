
using Bot.Models;
using Bot.Engine;
using System;
using Bot.Engine.Events;
using System.Linq;

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
            string openOrders = string.Join(',', engine.Broker.OpenOrders.Select(o => o.ToString()));
            string orderHistory = string.Join(',', engine.Broker.OrderHistory.Select(o => o.ToString()));

            Console.WriteLine($"Tick: {ticks}");
            Console.WriteLine($"Portfolio: {engine.Broker.Portfolio}");
            Console.WriteLine($"Portfolio Value: {engine.Broker.PortfolioValue()}");
            Console.WriteLine($"Open Orders: {openOrders}");
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

            string orderHistory = string.Join(',', engine.Broker.OrderHistory.Select(o => o.ToString() + "\n"));
            Console.WriteLine($"Order History: {orderHistory}");

            Console.WriteLine(output);
        }
    }
}
