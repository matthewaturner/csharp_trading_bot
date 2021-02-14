using Bot.DataStorage.Models;
using Bot.Trading;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Brokerage.Interfaces
{
    public interface IBroker
    {
        /// <summary>
        /// Send a trade to the brokerage.
        /// </summary>
        /// <returns></returns>
        public void ExecuteTrade(Trade trade);

        /// <summary>
        /// Execute Bulk trades
        /// </summary>
        /// <returns></returns>
        public void BulkExecuteTrade(List<Trade> trades);

    }
}
