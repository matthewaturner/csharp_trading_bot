using Bot.Engine;
using Bot.Engine.Events;
using Bot.Models;
using Bot.Models.Interfaces;
using System;
using System.Collections.Generic;

namespace Bot.Brokers
{
    public interface IBroker : ITickReceiver
    {
        /// <summary>
        /// Initializes the broker with custom arguments.
        /// </summary>
        /// <param name="args"></param>
        public void Initialize(ITradingEngine engine, string[] args);

        /// <summary>
        /// Gets the current portfolio value.
        /// </summary>
        /// <returns></returns>
        public double PortfolioValue();

        /// <summary>
        /// Gets the current portfolio cash value.
        /// </summary>
        /// <returns></returns>
        public double CashBalance();

        /// <summary>
        /// Gets the history of orders.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public IList<Order> GetOrderHistory(DateTime start, DateTime end);

        /// <summary>
        /// Places an order.
        /// </summary>
        /// <returns></returns>
        public string PlaceOrder(OrderRequest order);

        /// <summary>
        /// Gets the status of an order.
        /// </summary>
        public Order GetOrder(string orderId);

        /// <summary>
        /// Cancel an order if it hasn't been filled yet.
        /// </summary>
        /// <param name="order"></param>
        public void CancelOrder(string orderId);

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
        /// List of open orders.
        /// </summary>
        IList<Order> OpenOrders { get; }

        /// <summary>
        /// Order history.
        /// </summary>
        IList<Order> OrderHistory { get; }
    }
}
