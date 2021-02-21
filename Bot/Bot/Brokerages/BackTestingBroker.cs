﻿using Bot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models
{
    public class BackTestingBroker : IBroker
    {
        private ITicks ticks;

        public BackTestingBroker(ITicks ticks, double initialFunds)
        {
            this.ticks = ticks;
            Portfolio = new Portfolio(initialFunds);
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
        public string PlaceOrder(OrderRequest request)
        {
            Order order = new Order(request);

            if (!ticks.HasSymbol(order.Symbol))
            {
                throw new InvalidOrderException("Cannot place orders for symbols we aren't gathering prices for.");
            }

            order.OrderId = Guid.NewGuid().ToString();
            OpenOrders.Add(order);
            OrderHistory.Add(order);

            return order.OrderId;
        }

        /// <summary>
        /// Returns order history.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
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
            Order order = OpenOrders.FirstOrDefault();
            while (order != null)
            {
                OrderState state = PreviewOrder(order);
                if (state == OrderState.Rejected)
                {
                    order.State = OrderState.Rejected;
                }
                else
                {
                    switch (order.Type)
                    {
                        case OrderType.Buy:
                            Portfolio.Buy(order.Symbol, order.Quantity, ticks[order.Symbol].AdjOpen);
                            break;

                        case OrderType.Sell:
                            Portfolio.Sell(order.Symbol, order.Quantity, ticks[order.Symbol].AdjOpen);
                            break;
                    }

                    // mark order as filled and remove it from the open orders list,
                    // it will remain in the order history list
                    order.Fill(ticks[order.Symbol].AdjOpen, ticks[order.Symbol].DateTime);
                }

                OpenOrders.RemoveAt(0);
                order = OpenOrders.FirstOrDefault();
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
            Order order = OpenOrders.SingleOrDefault(order => string.CompareOrdinal(order.OrderId, orderId) == 0);

            if (order != null)
            {
                order.State = OrderState.Cancelled;
                OpenOrders.Remove(order);
            }
        }

        /// <summary>
        /// Basically just returns if an order is valid or not.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OrderState PreviewOrder(Order order)
        {
            double currentPrice = ticks[order.Symbol].AdjOpen;
            double orderPrice = currentPrice * order.Quantity;

            switch (order.Type)
            {
                // logic is the same for now
                case OrderType.Buy:
                    if (orderPrice > Portfolio.CashBalance)
                    {
                        return OrderState.Rejected;
                    }
                    break;

                case OrderType.Sell:
                    double netQuantity = Portfolio.HasPosition(order.Symbol) ?
                        Portfolio[order.Symbol].Quantity - order.Quantity :
                        -order.Quantity;

                    if (netQuantity < 0)
                    {
                        if (currentPrice * -netQuantity > Portfolio.CurrentValue(ticks, (t) => t.AdjOpen))
                        {
                            return OrderState.Rejected;
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            return OrderState.Open;
        }
    }
}
