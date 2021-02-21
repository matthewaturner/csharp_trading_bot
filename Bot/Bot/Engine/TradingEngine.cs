using Bot.DataStorage;
using Bot.Models;
using Bot.Strategies;
using Bot.Trading;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Trading
{
    public class TradingEngine : ITradingEngine
    {
        private ITickStorage TickStorage;
        private IStrategy Strategy;
        private IList<Tick> TickData;
        private IBroker Broker;

        public TradingEngine(
            ITickStorage tickStorage,
            IStrategy strategy,
            IBroker broker)
        {
            this.TickStorage = tickStorage ?? throw new ArgumentNullException(nameof(tickStorage));
            this.Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }

        /// <summary>
        /// BackFill Data for any dates previous to current date
        /// </summary>
        public void InitializeTickData(string ticker, DateTime startDate, DateTime endDate, TickInterval tickInterval)
        {
            this.TickData = this.TickStorage.GetTicksAsync(ticker, tickInterval, startDate, endDate).Result;
        }

        /// <summary>
        /// Run the <see cref="TradingEngine.Strategy"/> on <see cref="Tick"/> between the provided DateTimes based on the provided <see cref="Bot.Brokerages.TickInterval"/>
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="tickInterval"></param>
        public void Run(string ticker, DateTime startDate, DateTime endDate, TickInterval tickInterval)
        {
            this.InitializeTickData(ticker, startDate, endDate, tickInterval);
            foreach (var tick in this.TickData)
            {
                this.Strategy.OnTick(tick);
                this.Broker.OnTick(tick);
            }
        }
    }
}
