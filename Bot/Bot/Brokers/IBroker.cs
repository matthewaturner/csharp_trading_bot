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
        void Initialize(ITradingEngine engine, string[] args);

        /// <summary>
        /// Gets the current portfolio value.
        /// </summary>
        /// <returns></returns>
        double GetPortfolioValue();

        /// <summary>
        /// Gets the current portfolio cash value.
        /// </summary>
        /// <returns></returns>
        double GetCashBalance();

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
        IList<IOrder> GetOrdersByState(OrderState state);

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
