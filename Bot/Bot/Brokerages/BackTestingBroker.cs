using Bot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models
{
    public class BackTestingBroker : IBroker
    {

        public BackTestingBroker(double initialFunds)
        {
            Portfolio = new Portfolio(initialFunds);
            OpenOrders = new List<Order>();
            OrderHistory = new List<Order>();
        }

        public Portfolio Portfolio { get; private set; }

        public IList<Order> OpenOrders { get; private set; }

        public IList<Order> OrderHistory { get; private set; }

        public void PlaceOrder(Order order)
        {
            bool holdingSymbol = HoldingSymbol(order.Symbol);

            switch (order.Type)
            {
                case OrderType.BuyToOpen:
                    return;

                case OrderType.SellToClose:
                    if (!holdingSymbol)
                    {
                        throw new InvalidOrderException("Cannot close a position which is not held.");
                    }

                    return;
            }
        }

        public void GetQuote(string symbol)
        {
            throw new NotImplementedException();
        }

        public void GetTradeHistory(DateTime start, DateTime end)
        {
            throw new NotImplementedException();
        }

        public Order GetTradeStatus()
        {
            throw new NotImplementedException();
        }

        public void OnTick(Tick tick)
        {
            throw new NotImplementedException();
        }

        public Order GetOrder(string orderId)
        {
            throw new NotImplementedException();
        }

        public void CancelOrder(Order order)
        {
            throw new NotImplementedException();
        }

    }
}
