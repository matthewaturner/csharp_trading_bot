
using Bot.Models;
using Bot.Engine;
using System;
using Bot.Engine.Events;
using System.Linq;

namespace Bot.Analyzers.Loggers
{
    public class ConsoleLogger : IAnalyzer, ILogReceiver, ITerminateReceiver
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
        /// Sets colors based on log level.
        /// </summary>
        /// <param name="level"></param>
        private void SetColors(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Verbose:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Information:
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }

        /// <summary>
        /// When a log event is triggered.
        /// </summary>
        /// <param name="log"></param>
        public void OnLog(string caller, string message, LogLevel level = LogLevel.Information)
        {
            SetColors(level);
            Console.WriteLine($"[{DateTime.Now.ToString("O")}] {caller}: {message}");
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
