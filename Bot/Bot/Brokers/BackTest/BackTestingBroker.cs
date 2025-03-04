using Bot.Brokers.BackTest.Models;
using Bot.Events;
using Bot.Models;
using Bot.Models.Broker;
using Bot.Models.Interfaces;
using Bot.Models.MarketData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Brokers.BackTest;

public class BackTestingBroker : BrokerBase, IBroker, IMarketDataReceiver
{
    // private objects
    private BackTestAccount account;
    private IList<BackTestPosition> positions;
    private IList<BackTestOrder> openOrders;
    private IList<BackTestOrder> allOrders;
    private ExecutionMode executionMode;

    // private funcs
    private ILogger Logger => GlobalConfig.GlobalLogger;

    private Bar CurrentBar(string symbol) => Engine.DataSource.GetLatestBar(symbol);

    /// <summary>
    /// Dependency injection constructor.
    /// </summary>
    public BackTestingBroker(double initialFunds, ExecutionMode executionMode = ExecutionMode.OnCurrentBar)
    {
        this.account = new BackTestAccount(initialFunds);
        this.positions = new List<BackTestPosition>();
        this.openOrders = new List<BackTestOrder>();
        this.allOrders = new List<BackTestOrder>();
        this.executionMode = executionMode;
    }

    #region Events ===================================================================================================

    /// <summary>
    /// Execute orders at the next price.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void OnEvent(object sender, MarketDataEvent e)
    {
        if (executionMode == ExecutionMode.OnNextBar)
        {
            ExecuteAllOrders();
        }
    }

    #endregion =======================================================================================================

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
        // update total value then return account
        account.TotalValue = account.Cash + positions.Sum(p => p.Quantity * DataSource.GetLatestBar(p.Symbol).AdjClose);
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
    /// Placing an order just puts it into the open orders list.
    /// </summary>
    /// <param name="order"></param>
    public IOrder PlaceOrder(IOrderRequest request)
    {
        BackTestOrder order = new BackTestOrder(request);
        order.OrderId = Guid.NewGuid().ToString();
        order.PlacementTime = DateTime.Now;
        openOrders.Add(order);
        allOrders.Add(order);

        if (executionMode == ExecutionMode.OnCurrentBar)
        {
            ExecuteAllOrders();
        }

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
        // todo: multibars
        double currentPrice = Engine.DataSource.GetLatestBar(order.Symbol).AdjClose;
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
    /// Execute all of the outstanding orders at the current bar price.
    /// </summary>
    /// <param name="bar"></param>
    private void ExecuteAllOrders()
    {
        BackTestOrder order = openOrders.FirstOrDefault();
        Bar bar = CurrentBar(order.Symbol);

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
                        ApplyTransaction(order.Symbol, order.Quantity, bar.AdjClose);
                        break;

                    case OrderType.MarketSell:
                        ApplyTransaction(order.Symbol, -order.Quantity, bar.AdjClose);
                        break;
                }

                // mark order as filled and remove it from the open orders list,
                // it will remain in the order history list
                order.Fill(bar.AdjClose, bar.Timestamp);
                Logger.LogInformation($"Order filled. {order}");
            }

            openOrders.RemoveAt(0);
            order = openOrders.FirstOrDefault();
        }
    }

    /// <summary>
    /// Buys a security.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="quantity">Negative for sells.</param>
    /// <param name="price"></param>
    private void ApplyTransaction(string symbol, double quantity, double price)
    {
        if (quantity == 0)
        {
            return;
        }

        BackTestPosition currentPosition = positions.FirstOrDefault(pos => pos.Symbol.Equals(symbol));

        if (currentPosition != null)
        {
            currentPosition.Quantity += quantity;
            account.Cash -= price * quantity;
        }
        else
        {
            positions.Add(new BackTestPosition(symbol, quantity));
            account.Cash -= price * quantity;
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
            Math.Abs(position.Quantity));
        return PlaceOrder(order);
    }
}
