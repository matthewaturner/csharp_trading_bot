using Bot.Exceptions;
using Bot.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using Bot.Models.Interfaces;
using Bot.Brokers.BackTest.Models;
using Bot.Models;

namespace Bot.Brokers.BackTest
{
    public class BackTestingBroker : IBroker
    {
        private IMultiTick ticks;
        private ITradingEngine engine;
        private BackTestAccount account;
        private IList<BackTestPosition> positions;

        public BackTestingBroker()
        { }

        /// <summary>
        /// Initialize with custom arguments.
        /// </summary>
        /// <param name="args"></param>
        public void Initialize(ITradingEngine engine, string[] args)
        {
            double initialFunds = double.Parse(args[0]);

            this.engine = engine;
            ticks = engine.Ticks;

            account = new BackTestAccount(initialFunds);
            positions = new List<BackTestPosition>();

            OpenOrders = new List<Order>();
            OrderHistory = new List<Order>();
        }

        /// <summary>
        /// Gets account object.
        /// </summary>
        public IAccount GetAccount()
        {
            return account;
        }

        /// <summary>
        /// Positions held.
        /// </summary>
        public IList<IPosition> GetPositions()
        {
            return positions.ToList<IPosition>();
        }

        public IPosition GetPosition(string symbol)
        {
            return positions.FirstOrDefault(pos => pos.Symbol.Equals(symbol));
        }

        /// <summary>
        /// Get open orders.
        /// </summary>
        public IList<Order> OpenOrders { get; private set; }

        /// <summary>
        /// Get order history.
        /// </summary>
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
            order.PlacementTime = ticks[order.Symbol].DateTime;
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
        /// <param name="_"></param>
        public void OnTick(IMultiTick ticks)
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
                        case OrderType.MarketBuy:
                            Buy(order.Symbol, order.Quantity, ticks[order.Symbol].AdjOpen);
                            break;

                        case OrderType.MarketSell:
                            Sell(order.Symbol, order.Quantity, ticks[order.Symbol].AdjOpen);
                            break;
                    }

                    // mark order as filled and remove it from the open orders list,
                    // it will remain in the order history list
                    order.Fill(ticks[order.Symbol].AdjOpen, ticks[order.Symbol].DateTime);
                }

                OpenOrders.RemoveAt(0);
                order = OpenOrders.FirstOrDefault();
            }

            UpdateAccountValue(ticks);
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
                case OrderType.MarketBuy:
                    if (orderPrice > account.CashBalance)
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
        /// Gets the current portfolio value.
        /// </summary>
        /// <returns></returns>
        public double PortfolioValue()
        {
            return account.TotalValue;
        }

        /// <summary>
        /// Gets the current cash balance.
        /// </summary>
        /// <returns></returns>
        public double CashBalance()
        {
            return account.CashBalance;
        }

        /// <summary>
        /// Updates the total account value based on latest prices.
        /// </summary>
        /// <param name="tick"></param>
        private void UpdateAccountValue(IMultiTick tick)
        {
            double total = account.CashBalance;
            foreach (IPosition pos in positions)
            {
                total += pos.Quantity * tick[pos.Symbol].AdjClose;
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
                account.CashBalance -= price * quantity;

                if (currentPosition.Quantity == 0)
                {
                    positions.Remove(currentPosition);
                }
            }
            else
            {
                positions.Add(new BackTestPosition(symbol, quantity));
                account.CashBalance -= price * quantity;
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
                account.CashBalance += price * quantity;

                if (currentPosition.Quantity == 0)
                {
                    positions.Remove(currentPosition);
                }
            }
            else
            {
                positions.Add(new BackTestPosition(symbol, -quantity));
                account.CashBalance += price * quantity;
            }
        }
    }
}
