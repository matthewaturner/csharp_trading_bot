using Bot;
using Bot.Models;
using Bot.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace BrokerUnitTests
{
    [TestClass]
    public class BackTestingBrokerUnitTests
    {
        Ticks ticks;
        Mock<ITradingEngine> mockEngine;

        [TestInitialize]
        public void Setup()
        {
            mockEngine = new Mock<ITradingEngine>();

            ticks = new Ticks(new string[] { "MSFT", "GME", "AMC" });

            var msftTick = new Tick("MSFT", TickInterval.Day, DateTime.Now, 245.03, 246.13, 242.92, 243.70, 243.70, 26708200);
            var gmeTick = new Tick("GME", TickInterval.Day, DateTime.Now, 52.22, 53.50, 49.04, 49.51, 49.51, 8140700);
            var amcTick = new Tick("AMC", TickInterval.Day, DateTime.Now, 6.03, 6.05, 5.49, 5.65, 5.65, 60690200);

            Tick[] latestTicks = new Tick[] { msftTick, gmeTick, amcTick };
            ticks.Update(latestTicks);

            mockEngine.Setup(m => m.Ticks).Returns(ticks);
        }

        [TestMethod]
        public void BuyOrderSucceeds()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsTrue(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(10.0, broker.Portfolio["MSFT"].Quantity);
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void SellOrderSucceeds()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Sell, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsTrue(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(-10.0, broker.Portfolio["MSFT"].Quantity);
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void BuyOrderInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "1000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsTrue(!broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void ShortSaleInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Sell, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "1000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsTrue(!broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }


        [TestMethod]
        public void SellOrderInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Sell, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "1000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsTrue(!broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void CancelOrderDoesNotExecute()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            broker.CancelOrder(orderId);

            broker.OnTick(ticks);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Cancelled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void BuyThenSellAllSucceeds()
        {
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            OrderRequest buyRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.Sell, "MSFT", 10.0, 31.0);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick(ticks);
            Assert.IsTrue(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(10.0, broker.Portfolio["MSFT"].Quantity);
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick(ticks);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.OrderHistory.Count);
        }

        [TestMethod]
        public void BuyThenShortSellSucceeds()
        {
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            OrderRequest buyRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.Sell, "MSFT", 15.0, 31.0);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick(ticks);
            Assert.IsTrue(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(10.0, broker.Portfolio["MSFT"].Quantity);
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick(ticks);
            Assert.IsTrue(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(-5, broker.Portfolio["MSFT"].Quantity);
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen).Round());
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.OrderHistory.Count);
        }

        [TestMethod]
        public void BuySellProfitSucceeds()
        {
            Ticks t = new Ticks(new string[] { "TEST" });

            var testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 10, 10, 10, 10, 10, 100);
            t.Update(new Tick[] { testTick });

            // make sure engine returns our different ticks object
            mockEngine.Setup(m => m.Ticks).Returns(t);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "100" });

            OrderRequest buyRequest = new OrderRequest(OrderType.Buy, "TEST", 10.0, 10.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.Sell, "TEST", 10.0, 10.0);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(100, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick(t);
            Assert.IsTrue(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(10.0, broker.Portfolio["TEST"].Quantity);
            Assert.AreEqual(100, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            // update the prices
            testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 11, 11, 11, 11, 11, 100);
            t.Update(new Tick[] { testTick });

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick(t);
            Assert.IsFalse(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(110, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen).Round());
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.OrderHistory.Count);
        }

        [TestMethod]
        public void SellBuyProfitSucceeds()
        {
            Ticks t = new Ticks(new string[] { "TEST" });

            var testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 10, 10, 10, 10, 10, 100);
            t.Update(new Tick[] { testTick });

            mockEngine.Setup(m => m.Ticks).Returns(t);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "100" });

            OrderRequest buyRequest = new OrderRequest(OrderType.Buy, "TEST", 10.0, 10.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.Sell, "TEST", 10.0, 10.0);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(100, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick(t);
            Assert.IsTrue(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(-10.0, broker.Portfolio["TEST"].Quantity);
            Assert.AreEqual(100, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);

            // update the prices
            testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 9, 9, 9, 9, 9, 100);
            t.Update(new Tick[] { testTick });

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick(t);
            Assert.IsFalse(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(110, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen).Round());
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);
            Assert.AreEqual(2, broker.OrderHistory.Count);
        }
    }
}
