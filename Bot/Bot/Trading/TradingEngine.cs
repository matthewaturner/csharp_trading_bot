using Bot.DataStorage;
using Bot.Interfaces.Trading;
using Bot.Trading.Interfaces;
using System;
using System.Collections.Generic;

namespace Bot.Trading
{
    public class TradingEngine : ITradingEngine
    {
        private ITickStorage TickStorage;

        private List<IStrategy> Strategies;

        public TradingEngine(ITickStorage tickStorage, List<IStrategy> strategies)
        {
            this.TickStorage = tickStorage != null ? tickStorage : throw new ArgumentNullException(nameof(tickStorage));
            this.Strategies = strategies != null ? strategies : throw new ArgumentNullException(nameof(strategies));
        }
        

        public void RunStrategies()
        {
            //calculate indicators
            //loop through the strategies to get trade signals
            //send trades to broker
        }
    }
}
