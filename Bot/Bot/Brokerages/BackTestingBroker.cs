using Bot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models
{
    public class BackTestingBroker : IBroker
    {
        private ICurrentTicks currentTicks;

        public BackTestingBroker(ICurrentTicks currentTicks, double initialFunds)
        {
            this.currentTicks = currentTicks;
            Portfolio = new Portfolio(currentTicks, initialFunds);
            OpenOrders = new List<Order>();
            OrderHistory = new List<Order>();
        }

        public Portfolio Portfolio { get; private set; }

        public IList<Order> OpenOrders { get; private set; }

        public IList<Order> OrderHistory { get; private set; }

        /// <summary>
        /// Placing an order just puts it into the open orders list.
        /// </summary>
        /// <param name="order"></param>
        public void PlaceOrder(Order order)
        {
            if (!currentTicks.HasSymbol(order.Symbol))
            {
                throw new InvalidOrderException("Cannot place orders for symbols we aren't gathering prices for.");
            }

            order.OrderId = Guid.NewGuid().ToString();
            OpenOrders.Add(order);
            OrderHistory.Add(order);
        }

        public IList<Order> GetOrderHistory(DateTime start, DateTime end)
        {
            return OrderHistory.Where(order => order.PlacementTime >= start && order.PlacementTime < end).ToList();
        }

        /// <summary>
        /// Open orders execute at the open price of the next tick.
        /// </summary>
        /// <param name="tick"></param>
        public void OnTick()
        {
            foreach (Order order in OpenOrders)
            {
                if (!Portfolio.ValidateOrder(order, currentTicks[order.Symbol].Close, out string message))
                {
                    throw new InvalidOrderException($"Order could not be validated. {message}");
                }

                order.Fill(currentTicks[order.Symbol].Open, currentTicks[order.Symbol].DateTime);
                Portfolio.ExecuteOrder(order, order.ExecutionPrice);
                OpenOrders.Remove(order);
            }
        }

        /// <summary>
        /// Gets the status of an order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public Order GetOrder(string orderId)
        {
            return OrderHistory.FirstOrDefault(order => string.CompareOrdinal(order.OrderId, orderId) == 0);
        }

        /// <summary>
        /// Cancels an order if it hasn't been filled yet.
        /// </summary>
        /// <param name="orderId"></param>
        public void CancelOrder(string orderId)
        {
            Order order = OpenOrders.SingleOrDefault(order => string.CompareOrdinal(order.OrderId, orderId) != 0);

            if (order != null)
            {
                OpenOrders.Remove(order);
            }
        }
    }
}
