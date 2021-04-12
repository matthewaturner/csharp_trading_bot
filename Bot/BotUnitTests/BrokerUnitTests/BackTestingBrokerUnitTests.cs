using Bot.Models;
using Bot.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Bot.Brokers.BackTest;
using System.Linq;

namespace BrokerUnitTests
{
    [TestClass]
    public class BackTestingBrokerUnitTests
    {
        MultiTick ticks;
        Mock<ITradingEngine> mockEngine;

        [TestInitialize]
        public void Setup()
        {
            mockEngine = new Mock<ITradingEngine>();

            ticks = new MultiTick(new string[] { "MSFT", "GME", "AMC" });

            var msftTick = new Tick("MSFT", TickInterval.Day, DateTime.Now, 245.03, 246.13, 242.92, 243.70, 243.70, 26708200);
            var gmeTick = new Tick("GME", TickInterval.Day, DateTime.Now, 50.0, 50.0, 50.0, 50.0, 50.0, 8140700); // same open and close
            var amcTick = new Tick("AMC", TickInterval.Day, DateTime.Now, 6.0, 6.0, 6.0, 7.0, 7.0, 60690200); // open and close off by $1

            Tick[] latestTicks = new Tick[] { msftTick, gmeTick, amcTick };
            ticks.Update(latestTicks);

            mockEngine.Setup(m => m.Ticks).Returns(ticks);
        }

        [TestMethod]
        public void BuyOrderSucceeds()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketBuy, "GME", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsFalse(broker.GetPositions().Any(pos => pos.Symbol.Equals("GME")));
            Assert.AreEqual(3000, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOrdersByState(OrderState.Open).Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsNotNull(broker.GetPosition("GME"));
            Assert.AreEqual(10.0, broker.GetPosition("GME").Quantity);
            Assert.AreEqual(3000, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOrdersByState(OrderState.Open).Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void SellOrderSucceeds()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketSell, "GME", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsNull(broker.GetPosition("GME"));
            Assert.AreEqual(3000, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsNotNull(broker.GetPosition("GME"));
            Assert.AreEqual(-10.0, broker.GetPosition("GME").Quantity);
            Assert.AreEqual(3000, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void BuyOrderInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketBuy, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "1000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(1000, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(1000, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void ShortSaleInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketSell, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "1000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(1000, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(1000, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }


        [TestMethod]
        public void SellOrderInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketSell, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "1000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(1000, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);

            // execution happens on ticks
            broker.OnTick(ticks);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(1000, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void CancelOrderDoesNotExecute()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketBuy, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            string orderId = broker.PlaceOrder(orderRequest);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(3000, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            broker.CancelOrder(orderId);

            broker.OnTick(ticks);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(3000, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Cancelled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void BuyThenSellAllSucceeds()
        {
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            OrderRequest buyRequest = new OrderRequest(OrderType.MarketBuy, "AMC", 10.0, 25.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.MarketSell, "AMC", 10.0, 31.0);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.IsNull(broker.GetPosition("AMC"));
            Assert.AreEqual(3000, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick(ticks);
            Assert.IsNotNull(broker.GetPosition("AMC"));
            Assert.AreEqual(10.0, broker.GetPosition("AMC").Quantity);
            Assert.AreEqual(3010, broker.GetAccount().TotalValue); // when we calculate based on close prices it is inflated
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick(ticks);
            Assert.IsNull(broker.GetPosition("AMC"));
            Assert.AreEqual(3000.0, broker.GetAccount().TotalValue); // sold at the same price
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.GetAllOrders().Count);
        }

        [TestMethod]
        public void BuyThenShortSellSucceeds()
        {
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "3000" });

            OrderRequest buyRequest = new OrderRequest(OrderType.MarketBuy, "AMC", 10.0, 25.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.MarketSell, "AMC", 15.0, 31.0);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.IsNull(broker.GetPosition("AMC"));
            Assert.AreEqual(3000, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick(ticks);
            Assert.IsNotNull(broker.GetPosition("AMC"));
            Assert.AreEqual(10.0, broker.GetPosition("AMC").Quantity);
            Assert.AreEqual(3010, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick(ticks);
            Assert.IsNotNull(broker.GetPosition("AMC"));
            Assert.AreEqual(-5, broker.GetPosition("AMC").Quantity);
            Assert.AreEqual(2995.0, broker.GetAccount().TotalValue); // calculated based on close prices, we were short and it went up
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.GetAllOrders().Count);
        }

        [TestMethod]
        public void BuySellProfitSucceeds()
        {
            MultiTick t = new MultiTick(new string[] { "TEST" });

            var testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 10, 10, 10, 10, 10, 100);
            t.Update(new Tick[] { testTick });

            // make sure engine returns our different ticks object
            mockEngine.Setup(m => m.Ticks).Returns(t);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "100" });

            OrderRequest buyRequest = new OrderRequest(OrderType.MarketBuy, "TEST", 10.0, 10.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.MarketSell, "TEST", 10.0, 10.0);

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.IsNull(broker.GetPosition("TEST"));
            Assert.AreEqual(100, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick(t);
            Assert.IsNotNull(broker.GetPosition("TEST"));
            Assert.AreEqual(10.0, broker.GetPosition("TEST").Quantity);
            Assert.AreEqual(100, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            // update the prices
            testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 11, 11, 11, 11, 11, 100);
            t.Update(new Tick[] { testTick });

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick(t);
            Assert.IsNull(broker.GetPosition("TEST"));
            Assert.AreEqual(110, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.GetAllOrders().Count);
        }

        [TestMethod]
        public void SellBuyProfitSucceeds()
        {
            MultiTick t = new MultiTick(new string[] { "TEST" });

            var testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 10, 10, 10, 10, 10, 100);
            t.Update(new Tick[] { testTick });

            mockEngine.Setup(m => m.Ticks).Returns(t);
            BackTestingBroker broker = new BackTestingBroker();
            broker.Initialize(mockEngine.Object, new string[] { "100" });

            OrderRequest buyRequest = new OrderRequest(OrderType.MarketBuy, "TEST", 10.0, 10.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.MarketSell, "TEST", 10.0, 10.0);

            string sellOrderId = broker.PlaceOrder(sellRequest);
            Assert.IsNull(broker.GetPosition("TEST"));
            Assert.AreEqual(100, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.OnTick(t);
            Assert.IsNotNull(broker.GetPosition("TEST"));
            Assert.AreEqual(-10.0, broker.GetPosition("TEST").Quantity);
            Assert.AreEqual(100, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);

            // update the prices
            testTick = new Tick("TEST", TickInterval.Day, DateTime.Now, 9, 9, 9, 9, 9, 100);
            t.Update(new Tick[] { testTick });

            string buyOrderId = broker.PlaceOrder(buyRequest);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.OnTick(t);
            Assert.IsNull(broker.GetPosition("TEST"));
            Assert.AreEqual(110, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);
            Assert.AreEqual(2, broker.GetAllOrders().Count);
        }
    }
}
