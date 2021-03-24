﻿using Bot.Engine;
using System;
using System.Collections.Generic;

namespace Bot.Models
{
    public interface IBroker
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
        public double CashValue();

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
    }
}
