using System;
using System.Collections.Generic;

namespace Bot.Models
{
    public interface IBroker
    {
        /// <summary>
        /// Portfolio held by the broker.
        /// </summary>
        Portfolio Portfolio { get; }

        /// <summary>
        /// List of open orders.
        /// </summary>
        IList<Order> OpenOrders { get; }

        /// <summary>
        /// Order history.
        /// </summary>
        IList<Order> OrderHistory { get; }

        /// <summary>
        /// Gets the status of an order.
        /// </summary>
        public Order GetOrder(string orderId);

        /// <summary>
        /// Send a trade to the brokerage.
        /// </summary>
        /// <returns></returns>
        public void PlaceOrder(Order order);

        /// <summary>
        /// Cancel a trade.
        /// </summary>
        /// <param name="trade"></param>
        public void CancelOrder(Order order);

        /// <summary>
        /// Gets a quote for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        public void GetQuote(string symbol);

        /// <summary>
        /// Gets tick data from the trade engine to update
        /// the portfolio with.
        /// </summary>
        public void OnTick(Tick tick);
    }
}
