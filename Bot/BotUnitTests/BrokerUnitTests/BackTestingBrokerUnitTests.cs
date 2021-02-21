using Bot;
using Bot.Engine;
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
            ticks = new Ticks();
            ticks.Initialize(new string[] { "GME", "MSFT", "AMC" });

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
        public void BuyOrderSucceeds()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            IBroker broker = new BackTestingBroker(ticks);
            broker.Portfolio.AddFunds(3000);

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            // execution happens on ticks
            broker.OnTick();
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
            IBroker broker = new BackTestingBroker(ticks);
            broker.Portfolio.AddFunds(3000);

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            // execution happens on ticks
            broker.OnTick();
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
            IBroker broker = new BackTestingBroker(ticks);
            broker.Portfolio.AddFunds(1000);

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);

            // execution happens on ticks
            broker.OnTick();
            Assert.IsTrue(!broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void ShortSaleInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Sell, "MSFT", 10.0, 25.0);
            IBroker broker = new BackTestingBroker(ticks);
            broker.Portfolio.AddFunds(1000);

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);

            // execution happens on ticks
            broker.OnTick();
            Assert.IsTrue(!broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }


        [TestMethod]
        public void SellOrderInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Sell, "MSFT", 10.0, 25.0);
            IBroker broker = new BackTestingBroker(ticks);
            broker.Portfolio.AddFunds(1000);

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);

            // execution happens on ticks
            broker.OnTick();
            Assert.IsTrue(!broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(1000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void CancelOrderDoesNotExecute()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            IBroker broker = new BackTestingBroker(ticks);
            broker.Portfolio.AddFunds(3000);

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            broker.CancelOrder(orderId);

            broker.OnTick();
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Cancelled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void BuyThenSellAllSucceeds()
        {
            IBroker broker = new BackTestingBroker(ticks);
            broker.Portfolio.AddFunds(3000);

            OrderRequest buyRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.Sell, "MSFT", 10.0, 31.0);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick();
            Assert.IsTrue(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(10.0, broker.Portfolio["MSFT"].Quantity);
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick();
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.OrderHistory.Count);
        }

        [TestMethod]
        public void BuyThenShortSellSucceeds()
        {
            IBroker broker = new BackTestingBroker(ticks);
            broker.Portfolio.AddFunds(3000);

            OrderRequest buyRequest = new OrderRequest(OrderType.Buy, "MSFT", 10.0, 25.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.Sell, "MSFT", 15.0, 31.0);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick();
            Assert.IsTrue(broker.Portfolio.HasPosition("MSFT"));
            Assert.AreEqual(10.0, broker.Portfolio["MSFT"].Quantity);
            Assert.AreEqual(3000, broker.Portfolio.CurrentValue(ticks, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick();
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
            Ticks t = new Ticks();
            t.Initialize(new string[] { "TEST" });

            var testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 10, 10, 10, 10, 10, 100);
            Dictionary<string, Tick> latestTicks = new Dictionary<string, Tick>();
            latestTicks.Add("TEST", testTick);
            t.Update(latestTicks);

            IBroker broker = new BackTestingBroker(t);
            broker.Portfolio.AddFunds(100);

            OrderRequest buyRequest = new OrderRequest(OrderType.Buy, "TEST", 10.0, 10.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.Sell, "TEST", 10.0, 10.0);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(100, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick();
            Assert.IsTrue(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(10.0, broker.Portfolio["TEST"].Quantity);
            Assert.AreEqual(100, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            // update the prices
            testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 11, 11, 11, 11, 11, 100);
            latestTicks = new Dictionary<string, Tick>();
            latestTicks.Add("TEST", testTick);
            t.Update(latestTicks);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick();
            Assert.IsFalse(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(110, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen).Round());
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.OrderHistory.Count);
        }

        [TestMethod]
        public void SellBuyProfitSucceeds()
        {
            Ticks t = new Ticks();
            t.Initialize(new string[] { "TEST" });

            var testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 10, 10, 10, 10, 10, 100);
            Dictionary<string, Tick> latestTicks = new Dictionary<string, Tick>();
            latestTicks.Add("TEST", testTick);
            t.Update(latestTicks);

            IBroker broker = new BackTestingBroker(t);
            broker.Portfolio.AddFunds(100);

            OrderRequest buyRequest = new OrderRequest(OrderType.Buy, "TEST", 10.0, 10.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.Sell, "TEST", 10.0, 10.0);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.IsFalse(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(100, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen));
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick();
            Assert.IsTrue(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(-10.0, broker.Portfolio["TEST"].Quantity);
            Assert.AreEqual(100, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen));
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);

            // update the prices
            testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 9, 9, 9, 9, 9, 100);
            latestTicks = new Dictionary<string, Tick>();
            latestTicks.Add("TEST", testTick);
            t.Update(latestTicks);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.AreEqual(1, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick();
            Assert.IsFalse(broker.Portfolio.HasPosition("TEST"));
            Assert.AreEqual(110, broker.Portfolio.CurrentValue(t, (t) => t.AdjOpen).Round());
            Assert.AreEqual(0, broker.OpenOrders.Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);
            Assert.AreEqual(2, broker.OrderHistory.Count);
        }
    }
}
