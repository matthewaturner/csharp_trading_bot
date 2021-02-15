using Bot.Brokerages;
using Bot.DataStorage;
using Bot.Interfaces.Trading;
using Bot.Trading.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Trading
{
    public class TradingEngine : ITradingEngine
    {
        private ITickStorage TickStorage;

        private IStrategy Strategy;

        private string Ticker;

        private TickInterval TickInterval;

        private IList<Tick> TickData;

        public TradingEngine(
            ITickStorage tickStorage,
            IStrategy strategy,
            string ticker,
            TickInterval tickInterval)
        {
            this.TickStorage = tickStorage ?? throw new ArgumentNullException(nameof(tickStorage));
            this.Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
            this.Ticker = !string.IsNullOrEmpty(ticker) ? ticker : throw new ArgumentNullException(nameof(ticker));
            this.TickInterval = tickInterval;
        }

        /// <summary>
        /// BackFill Data for any dates previous to current date
        /// </summary>
        public void InitializeTickData(DateTime startDate, DateTime endDate)
        {
            this.TickData = this.TickStorage.GetTicksAsync(this.Ticker, this.TickInterval, startDate, endDate).Result;
        }

        /// <summary>
        /// Run the <see cref="TradingEngine.Strategy"/> on <see cref="Tick"/> between the provided DateTimes based on the provided <see cref="Bot.Brokerages.TickInterval"/>
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="tickInterval"></param>
        public void Run(DateTime startDate, DateTime endDate, TickInterval tickInterval)
        {
            this.TickInterval = tickInterval;
            this.InitializeTickData(startDate, endDate);
            this.TickData.OrderBy(f => f.DateTime).ToList().ForEach(t => this.Strategy.OnTick(t));         
        }

    }
}
