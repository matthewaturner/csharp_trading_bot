﻿using Bot.Exceptions;
using Bot.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using Bot.Models.Interfaces;
using Bot.Brokers.BackTest.Models;
using Bot.Models;
using Bot.Engine.Events;

namespace Bot.Brokers.BackTest
{
    public class BackTestingBroker : BrokerBase, IBroker, ITickReceiver
    {
        private BackTestAccount account;
        private IList<BackTestPosition> positions;
        private IList<BackTestOrder> openOrders;
        private IList<BackTestOrder> allOrders;

        /// <summary>
        /// Dependency injection constructor.
        /// </summary>
        public BackTestingBroker(
            double initialFunds)
        { 
            this.account = new BackTestAccount(initialFunds);
            this.positions = new List<BackTestPosition>();
            this.openOrders = new List<BackTestOrder>();
            this.allOrders = new List<BackTestOrder>();
        }

        private IMultiTick Ticks => Engine.Ticks;

        /// <summary>
        /// Gets asset information.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public IAssetInformation GetAssetInformation(string symbol)
        {
            return new BackTestAssetInformation(symbol);
        }

        /// <summary>
        /// Gets account object.
        /// </summary>
        public IAccount GetAccount()
        {
            return account;
        }

        /// <summary>
        /// Gets all positions held.
        /// </summary>
        public IList<IPosition> GetPositions()
        {
            return positions.ToList<IPosition>();
        }

        /// <summary>
        /// Gets the position held in some symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public IPosition GetPosition(string symbol)
        {
            return positions.FirstOrDefault(pos => pos.Symbol.Equals(symbol));
        }

        /// <summary>
        /// Gets the status of an order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public IOrder GetOrder(string orderId)
        {
            return allOrders.FirstOrDefault(order => string.CompareOrdinal(order.OrderId, orderId) == 0);
        }

        /// <summary>
        /// Gets all outstanding orders.
        /// </summary>
        /// <returns></returns>
        public IList<IOrder> GetOpenOrders()
        {
            return openOrders.ToList<IOrder>();
        }

        /// <summary>
        /// Gets all orders.
        /// </summary>
        /// <returns></returns>
        public IList<IOrder> GetAllOrders()
        {
            return allOrders.ToList<IOrder>();
        }

        /// <summary>
        /// Gets all orders in some state (open, filled, etc.)
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public IList<IOrder> QueryOrders(IEnumerable<string> symbols, OrderState state, DateTime after, DateTime until, int limit = 50)
        {
            return allOrders.Where(order => 
                symbols.Contains(order.Symbol, StringComparer.OrdinalIgnoreCase)
                && order.State == state
                && order.PlacementTime > after 
                && order.PlacementTime < until)
                .Take(limit).ToList<IOrder>();
        }

        /// <summary>
        /// Open orders execute at the open price of the next tick.
        /// </summary>
        /// <param name="_"></param>
        public void BaseOnTick(IMultiTick ticks)
        {
            BackTestOrder order = openOrders.FirstOrDefault();
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
                        case OrderType.MarketBuy:
                            Buy(order.Symbol, order.Quantity, ticks[order.Symbol].Close);
                            break;

                        case OrderType.MarketSell:
                            Sell(order.Symbol, order.Quantity, ticks[order.Symbol].Close);
                            break;
                    }

                    // mark order as filled and remove it from the open orders list,
                    // it will remain in the order history list
                    order.Fill(ticks[order.Symbol].Close, ticks[order.Symbol].DateTime);
                    Engine.Logger.LogInformation($"Order filled. {order}");
                }

                openOrders.RemoveAt(0);
                order = openOrders.FirstOrDefault();
            }

            UpdateAccountValue(ticks);
        }

        /// <summary>
        /// Placing an order just puts it into the open orders list.
        /// </summary>
        /// <param name="order"></param>
        public IOrder PlaceOrder(IOrderRequest request)
        {
            BackTestOrder order = new BackTestOrder(request);

            if (!Engine.Ticks.HasSymbol(order.Symbol))
            {
                throw new InvalidOrderException("Cannot place orders for symbols we aren't gathering prices for.");
            }

            order.OrderId = Guid.NewGuid().ToString();
            order.PlacementTime = Ticks[order.Symbol].DateTime;
            openOrders.Add(order);
            allOrders.Add(order);
            return order;
        }


        /// <summary>
        /// Cancels an order if it hasn't been filled yet.
        /// </summary>
        /// <param name="orderId"></param>
        public void CancelOrder(string orderId)
        {
            BackTestOrder order = openOrders.SingleOrDefault(order => string.CompareOrdinal(order.OrderId, orderId) == 0);

            if (order != null)
            {
                order.State = OrderState.Cancelled;
                openOrders.Remove(order);
            }
        }

        /// <summary>
        /// Basically just returns if an order is valid or not.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public OrderState PreviewOrder(BackTestOrder order)
        {
            double currentPrice = Ticks[order.Symbol].Open;
            double orderPrice = currentPrice * order.Quantity;

            switch (order.Type)
            {
                // logic is the same for now
                case OrderType.MarketBuy:
                    if (orderPrice > account.Cash)
                    {
                        return OrderState.Rejected;
                    }
                    break;

                case OrderType.MarketSell:

                    IPosition pos = positions.FirstOrDefault(pos => pos.Symbol.Equals(order.Symbol));
                    double netQuantity = pos != null ? pos.Quantity - order.Quantity : -order.Quantity;

                    if (netQuantity < 0)
                    {
                        if (currentPrice * -netQuantity > account.TotalValue)
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

        /// <summary>
        /// Updates the total account value based on latest prices.
        /// </summary>
        /// <param name="tick"></param>
        private void UpdateAccountValue(IMultiTick tick)
        {
            double total = account.Cash;
            foreach (IPosition pos in positions)
            {
                total += pos.Quantity * tick[pos.Symbol].Close;
            }

            account.TotalValue = total;
        }

        /// <summary>
        /// Buys a security.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        private void Buy(string symbol, double quantity, double price)
        {
            if (quantity == 0)
            {
                return;
            }

            BackTestPosition currentPosition = positions.FirstOrDefault(pos => pos.Symbol.Equals(symbol));

            if (currentPosition != null)
            {
                currentPosition.Quantity += quantity;
                currentPosition.Type = quantity > 0 ? PositionType.Long : PositionType.Short;
                account.Cash -= price * quantity;

                if (currentPosition.Quantity == 0)
                {
                    positions.Remove(currentPosition);
                }
            }
            else
            {
                positions.Add(new BackTestPosition(symbol, quantity));
                account.Cash -= price * quantity;
            }
        }

        /// <summary>
        /// Sells a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="quantity"></param>
        private void Sell(string symbol, double quantity, double price)
        {
            if (quantity == 0)
            {
                return;
            }

            BackTestPosition currentPosition = positions.FirstOrDefault(pos => pos.Symbol.Equals(symbol));

            if (currentPosition != null)
            {
                currentPosition.Quantity -= quantity;
                currentPosition.Type = quantity > 0 ? PositionType.Long : PositionType.Short;
                account.Cash += price * quantity;

                if (currentPosition.Quantity == 0)
                {
                    positions.Remove(currentPosition);
                }
            }
            else
            {
                positions.Add(new BackTestPosition(symbol, -quantity));
                account.Cash += price * quantity;
            }
        }

        /// <summary>
        /// Closes a position.
        /// </summary>
        /// <param name="symbol"></param>
        public IOrder ClosePosition(string symbol)
        {
            IPosition position = GetPosition(symbol);
            OrderRequest order = new OrderRequest(
                position.Quantity > 0 ? OrderType.MarketSell : OrderType.MarketBuy,
                symbol,
                Math.Abs(position.Quantity),
                Ticks[symbol].Close);
            return PlaceOrder(order);
        }
    }
}
