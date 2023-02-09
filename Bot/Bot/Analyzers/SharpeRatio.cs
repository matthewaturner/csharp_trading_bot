using Bot.Models;
using Bot.Engine;
using System.Collections.Generic;
using System.Linq;
using Bot.Engine.Events;

namespace Bot.Analyzers
{
    public class SharpeRatio : IAnalyzer, ITickReceiver
    {
        public ITradingEngine engine;
        public double riskFreeRate;
        public double previousValue;
        public IList<double> returns;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public void Initialize(ITradingEngine engine, string[] args)
        {
            riskFreeRate = double.Parse(args[0]);

            this.engine = engine;
            previousValue = double.NaN;
            returns = new List<double>();
        }

        /// <summary>
        /// When a tick is received.
        /// </summary>
        /// <param name="ticks"></param>
        public void BaseOnTick(IMultiBar _)
        {
            double currentValue = engine.Broker.GetAccount().TotalValue;

            if (double.IsNaN(previousValue))
            {
                previousValue = currentValue;
                return;
            }

            returns.Add((currentValue - previousValue) / previousValue);
            previousValue = currentValue;
        }

        /// <summary>
        /// Write the value of the analyzer.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Sharpe Ratio: {CalculateValue()} " +
                $"Average Daily Return: {returns.Average()}";

        }

        /// <summary>
        /// Calculates sharpe ratio from returns.
        /// </summary>
        private double CalculateValue()
        {
            return (returns.Average() - riskFreeRate) / Helpers.StandardDeviation(returns);
        }
    }
}
