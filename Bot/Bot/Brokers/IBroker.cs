﻿using Bot.Engine;
using Bot.Models;
using Bot.Models.Interfaces;
using System;
using System.Collections.Generic;
using Bot.Events;

namespace Bot.Brokers
{
    public interface IBroker : IInitialize
    {
        /// <summary>
        /// Gets information for an asset like whether it is easy to borrow.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        IAssetInformation GetAssetInformation(string symbol);

        /// <summary>
        /// Places an order.
        /// </summary>
        /// <returns>Order id.</returns>
        IOrder PlaceOrder(IOrderRequest order);

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
        /// Closes a position in a given symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        IOrder ClosePosition(string symbol);

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