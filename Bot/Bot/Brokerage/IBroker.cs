using Bot.Trading;
using System.Collections.Generic;

namespace Bot.Models
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
