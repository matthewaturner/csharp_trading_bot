using Bot.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BrokerUnitTests
{
    [TestClass]
    public class BackTestingBrokerUnitTests
    {
        Ticks ticks;

        [TestInitialize]
        public void Setup()
        {
            ticks = new Ticks(new string[] { "GME", "MSFT", "AMC" });

            var msftTick = new Tick("MSFT", TickInterval.Day, DateTime.Now, 245.03, 246.13, 242.92, 243.70, 243.70, 26708200);
            var gmeTick = new Tick("GME", TickInterval.Day, DateTime.Now, 52.22, 53.50, 49.04, 49.51, 49.51, 8140700);
            var amcTick = new Tick("AMC", TickInterval.Day, DateTime.Now, 6.03, 6.05, 5.49, 5.65, 5.65, 60690200);

            Dictionary<string, Tick> latestTicks = new Dictionary<string, Tick>();
            latestTicks.Add("MSFT", msftTick);
            latestTicks.Add("GME", gmeTick);
            latestTicks.Add("AMC", amcTick);
            ticks.Update(latestTicks);
        }

        [TestMethod]
        public void PlaceOrderSucceeds()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            IBroker broker = new BackTestingBroker(ticks, 3000);

            broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));

            // execution happens on ticks
            broker.OnTick();
            Assert.IsTrue(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(10.0, broker.Portfolio["MSFT"].Quantity);
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
        }
        /*

        [TestMethod]
        public void BulkExecuteTrade_Buy_ThrowException()
        {
            var portfolio = new Portfolio(1000);
            var broker = new BackTestingBroker(portfolio);

            var orders = new List<Order>()
            {
                new Order(){Ticker = "GME", Price = 90, Units = 10, Type = OrderType.Buy },
                new Order(){Ticker = "AMC", Price = 10, Units = 20, Type = OrderType.Buy }
            };

            Assert.ThrowsException<Exception>(() => broker.BulkExecuteTrade(orders));
        }

        [TestMethod]
        public void ExecuteTrade_Buy()
        {
            var startingCash = 10000;
            var portfolio = new Portfolio(startingCash);
            var broker = new BackTestingBroker(portfolio);

            var order = new Order()
            {
                Ticker = "GME",
                Price = 100,
                Units = 10,
                Type = OrderType.Buy
            };

            broker.PlaceOrder(order);
            var expectedAvailableCash = startingCash - order.GetTradeValue();
            var expectedPostion = new Position("GME", PositionType.StockLong, 10, 100);
            var actualPosition = broker.Portfolio.CurrentPositions[expectedPostion.Name];

            Assert.AreEqual(expectedPostion.EntryPrice, actualPosition.EntryPrice);
            Assert.AreEqual(expectedPostion.Name, actualPosition.Name);
            Assert.AreEqual(expectedPostion.Size, actualPosition.Size);
            Assert.AreEqual(expectedPostion._Type, actualPosition._Type);
            Assert.AreEqual(expectedAvailableCash, broker.Portfolio.AvailableCash);
        }

        [TestMethod]
        public void BulkExecuteTrade_Buy()
        {
            var startingCash = 10000;
            var portfolio = new Portfolio(startingCash);
            var broker = new BackTestingBroker(portfolio);

            var orders = new List<Order>()
            {
                new Order(){Ticker = "GME", Price = 100, Units = 10, Type = OrderType.Buy },
                new Order(){Ticker = "AMC", Price = 10, Units = 20, Type = OrderType.Buy }
            };

            broker.BulkExecuteTrade(orders);
            var expectedAvailableCash = startingCash - orders[0].GetTradeValue() - orders[1].GetTradeValue();

            Assert.AreEqual(expectedAvailableCash, broker.Portfolio.AvailableCash);

            var expectedPositions = new Dictionary<string, Position>();
            expectedPositions.Add("GME",  new Position("GME", PositionType.StockLong, 10, 100));
            expectedPositions.Add("AMC", new Position("AMC", PositionType.StockLong, 20, 10));

            foreach (var expectedPosition in expectedPositions)
            {
                Assert.IsTrue(broker.Portfolio.CurrentPositions.ContainsKey(expectedPosition.Key));
                var actualPosition = broker.Portfolio.CurrentPositions[expectedPosition.Key];
                Assert.AreEqual(expectedPosition.Value.EntryPrice, actualPosition.EntryPrice);
                Assert.AreEqual(expectedPosition.Value.Name, actualPosition.Name);
                Assert.AreEqual(expectedPosition.Value.Size, actualPosition.Size);
                Assert.AreEqual(expectedPosition.Value._Type, actualPosition._Type);
            }          
        }

        [TestMethod]
        public void ExecuteTrade_Sell()
        {
            var startingCash = 1000;
            var portfolio = new Portfolio(startingCash);
            var broker = new BackTestingBroker(portfolio);

            var buy = new Order()
            {
                Ticker = "GME",
                Price = 100,
                Units = 10,
                Type = OrderType.Buy
            };

            broker.PlaceOrder(buy);

            var sell = new Order() 
            {
                Ticker = "GME",
                Price = 1000,
                Units = -10,
                Type = OrderType.Sell
            };

            broker.PlaceOrder(sell);
            var expectedAvailableCash = sell.GetTradeValue();

            Assert.AreEqual(expectedAvailableCash, broker.Portfolio.AvailableCash);
        }
        */
    }
}
