using Bot.Configuration;
using Bot.Engine;
using Bot.Models;
using Bot.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace Bot.Brokers
{
    public interface IBroker
    {
        /// <summary>
        /// Initializes the broker with custom arguments.
        /// </summary>
        /// <param name="args"></param>
        void Initialize(ITradingEngine engine, RunMode runMode, string[] args);

        /// <summary>
        /// Places an order.
        /// </summary>
        /// <returns>Order id.</returns>
        string PlaceOrder(IOrderRequest order);

        /// <summary>
        /// Cancel an order if it hasn't been filled yet.
        /// </summary>
        /// <param name="order"></param>
        void CancelOrder(string orderId);

        /// <summary>
        /// Get account information.
        /// </summary>
        IAccount GetAccount();

        /// <summary>
        /// Get current positions.
        /// </summary>
        /// <returns></returns>
        IList<IPosition> GetPositions();

        /// <summary>
        /// Gets a single position.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        IPosition GetPosition(string symbol);


        /// <summary>
        /// Gets the status of an order.
        /// </summary>
        IOrder GetOrder(string orderId);

        /// <summary>
        /// Get all orders in some state.
        /// </summary>
        IList<IOrder> QueryOrders(IEnumerable<string> symbols, OrderState state, DateTime after, DateTime until, int limit = 50);

        /// <summary>
        /// Gets all open orders.
        /// </summary>
        /// <returns></returns>
        IList<IOrder> GetOpenOrders();

        /// <summary>
        /// Order history.
        /// </summary>
        IList<IOrder> GetAllOrders();
    }
}
