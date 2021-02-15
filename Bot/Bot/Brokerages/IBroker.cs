using System;
using System.Collections.Generic;

namespace Bot.Models
{
    public interface IBroker
    {
        /// <summary>
        /// Gets portfolio of positions.
        /// </summary>
        /// <returns></returns>
        public Portfolio GetPortfolio();

        /// <summary>
        /// Gets history of trades over some time period.
        /// </summary>
        public void GetTradeHistory(DateTime start, DateTime end);

        /// <summary>
        /// Gets list of outstanding trades.
        /// </summary>
        public IList<Trade> GetOutstandingTrades();

        /// <summary>
        /// Gets the status of a trade.
        /// </summary>
        public Trade GetTradeStatus();

        /// <summary>
        /// Send a trade to the brokerage.
        /// </summary>
        /// <returns></returns>
        public void ExecuteTrade(Trade trade);

        /// <summary>
        /// Cancel a trade.
        /// </summary>
        /// <param name="trade"></param>
        public void CancelTrade(Trade trade);

        /// <summary>
        /// Gets a quote for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        public void GetQuote(string symbol);
    }
}
