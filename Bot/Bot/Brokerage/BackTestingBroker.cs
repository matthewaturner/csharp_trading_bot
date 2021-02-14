using Bot.Brokerage.Interfaces;
using Bot.Trading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brokerage
{
    public class BackTestingBroker : IBroker
    {
        public Portfolio Portfolio;

        public BackTestingBroker(Portfolio portfolio)
        {
            this.Portfolio = portfolio != null ? portfolio : throw new ArgumentNullException(nameof(portfolio));
        }

        public void BulkExecuteTrade(List<Trade> trades)
        {
            foreach (var trade in trades)
            {
                this.ExecuteTrade(trade);                
            }
        }

        public void ExecuteTrade(Trade trade)
        {
            if (trade.TradeType == TradeType.Buy)
            {
                if (trade.GetTradeValue() <= Portfolio.AvailableCash)
                {
                    Portfolio.EnterPosition(trade);
                }
                else if (trade.TradeType == TradeType.Buy)
                {
                    throw new Exception("Error: Not enough cash to execute trade");
                }
            }
            else if (trade.TradeType == TradeType.Sell)
            {
                Portfolio.ExitPosition(trade);
            }
        }
    }
}
