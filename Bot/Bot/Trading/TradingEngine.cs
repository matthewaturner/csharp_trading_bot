using Bot.Brokerage;
using Bot.Brokerage.Interfaces;
using Bot.DataStorage.Models;
using Bot.Interfaces.Trading;
using Bot.Trading.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Trading
{
    public class TradingEngine : ITradingEngine
    {
        private IBroker Broker;

        public TradingEngine(IBroker broker, List<IStrategy> strategies)
        {
            this.Broker = broker != null ? broker : throw new ArgumentNullException(nameof(broker));
        }

        public void RunStrategies()
        {
            //calculate indicators
            //loop through the strategies to get trade signals
            //send trades to broker
        }
    }
}
