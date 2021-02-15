using Bot.Brokerages;
using Bot.DataStorage;
using Bot.DataStorage.Models;
using Bot.Interfaces.Trading;
using Bot.Trading.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Trading
{
    public class TradingEngine : ITradingEngine
    {
        private ITickStorage TickStorage;

        private IStrategy Strategy;

        private IList<Tick> TickData;

        public TradingEngine(
            ITickStorage tickStorage,
            IStrategy strategy)
        {
            this.TickStorage = tickStorage ?? throw new ArgumentNullException(nameof(tickStorage));
            this.Strategy = strategy ?? throw new ArgumentNullException(nameof(strategy));
        }       

        public void Run(DateTime startDate, DateTime endDate, TimeSpan timeSpan)
        {
            var currentDate = startDate;
            while (currentDate < endDate)
            {
                if (currentDate > DateTime.UtcNow)
                {
                    TimeSpan waitTime = currentDate - DateTime.UtcNow;
                    Thread.Sleep(waitTime);
                }                   

            }            
        }

    }
}
