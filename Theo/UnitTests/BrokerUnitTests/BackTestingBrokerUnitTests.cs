using Theo.Models;
using Theo.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using Theo.Brokers.BackTest;
using System.Linq;

namespace BrokerUnitTests
{
    [TestClass]
    public class BackTestingBrokerUnitTests
    {
        double initialFunds = 10000;
        MultiBar bars;
        Mock<ITradingEngine> mockEngine;

        [TestInitialize]
        public void Setup()
        {
            mockEngine = new Mock<ITradingEngine>();

            bars = new MultiBar(new string[] { "MSFT", "GME", "AMC" });

            var msftBar = new Bar("MSFT", DataInterval.Day, DateTime.Now, 245.03, 246.13, 242.92, 243.70, 26708200);
            var gmeBar = new Bar("GME", DataInterval.Day, DateTime.Now, 50.0, 50.0, 50.0, 50.0, 8140700); // same open and close
            var amcBar = new Bar("AMC", DataInterval.Day, DateTime.Now, 6.0, 6.0, 6.0, 7.0, 60690200); // open and close off by $1

            Bar[] latestBars = new Bar[] { msftBar, gmeBar, amcBar };
            bars.Update(latestBars);

            mockEngine.Setup(m => m.Bars).Returns(bars);
        }

        [TestMethod]
        public void BuyOrderSucceeds()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketBuy, "GME", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            string orderId = broker.PlaceOrder(orderRequest).OrderId;
            Assert.IsFalse(broker.GetPositions().Any(pos => pos.Symbol.Equals("GME")));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            // execution happens on bars
            broker.BaseOnBar(bars);
            Assert.IsNotNull(broker.GetPosition("GME"));
            Assert.AreEqual(10.0, broker.GetPosition("GME").Quantity);
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void SellOrderSucceeds()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketSell, "GME", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            string orderId = broker.PlaceOrder(orderRequest).OrderId;
            Assert.IsNull(broker.GetPosition("GME"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            // execution happens on bars
            broker.BaseOnBar(bars);
            Assert.IsNotNull(broker.GetPosition("GME"));
            Assert.AreEqual(-10.0, broker.GetPosition("GME").Quantity);
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void BuyOrderInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketBuy, "MSFT", 1000.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            string orderId = broker.PlaceOrder(orderRequest).OrderId;
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);

            // execution happens on bars
            broker.BaseOnBar(bars);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void ShortSaleInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketSell, "MSFT", 1000.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            string orderId = broker.PlaceOrder(orderRequest).OrderId;
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);

            // execution happens on bars
            broker.BaseOnBar(bars);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }


        [TestMethod]
        public void SellOrderInsufficientCashFails()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketSell, "MSFT", 1000.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            string orderId = broker.PlaceOrder(orderRequest).OrderId;
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);

            // execution happens on bars
            broker.BaseOnBar(bars);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Rejected, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void CancelOrderDoesNotExecute()
        {
            OrderRequest orderRequest = new OrderRequest(OrderType.MarketBuy, "MSFT", 10.0, 25.0);
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            string orderId = broker.PlaceOrder(orderRequest).OrderId;
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(orderId).State);

            broker.CancelOrder(orderId);

            broker.BaseOnBar(bars);
            Assert.IsNull(broker.GetPosition("MSFT"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Cancelled, broker.GetOrder(orderId).State);
        }

        [TestMethod]
        public void BuyThenSellAllSucceeds()
        {
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            OrderRequest buyRequest = new OrderRequest(OrderType.MarketBuy, "AMC", 10.0, 25.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.MarketSell, "AMC", 10.0, 31.0);

            string buyOrderId = broker.PlaceOrder(buyRequest).OrderId;
            Assert.IsNull(broker.GetPosition("AMC"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.BaseOnBar(bars);
            Assert.IsNotNull(broker.GetPosition("AMC"));
            Assert.AreEqual(10.0, broker.GetPosition("AMC").Quantity);
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            string sellOrderId = broker.PlaceOrder(sellRequest).OrderId;
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.BaseOnBar(bars);
            Assert.IsNull(broker.GetPosition("AMC"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue); // sold at the same price
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.GetAllOrders().Count);
        }

        [TestMethod]
        public void BuyThenShortSellSucceeds()
        {
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            OrderRequest buyRequest = new OrderRequest(OrderType.MarketBuy, "AMC", 10.0, 25.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.MarketSell, "AMC", 15.0, 31.0);

            string buyOrderId = broker.PlaceOrder(buyRequest).OrderId;
            Assert.IsNull(broker.GetPosition("AMC"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.BaseOnBar(bars);
            Assert.IsNotNull(broker.GetPosition("AMC"));
            Assert.AreEqual(10.0, broker.GetPosition("AMC").Quantity);
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            string sellOrderId = broker.PlaceOrder(sellRequest).OrderId;
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.BaseOnBar(bars);
            Assert.IsNotNull(broker.GetPosition("AMC"));
            Assert.AreEqual(-5, broker.GetPosition("AMC").Quantity);
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.GetAllOrders().Count);
        }

        [TestMethod]
        public void BuySellProfitSucceeds()
        {
            MultiBar t = new MultiBar(new string[] { "TEST" });

            var testBar = new Bar("TEST", DataInterval.Day, DateTime.Now, 10, 10, 10, 10, 100);
            t.Update(new Bar[] { testBar });

            // make sure engine returns our different bars object
            mockEngine.Setup(m => m.Bars).Returns(t);
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            OrderRequest buyRequest = new OrderRequest(OrderType.MarketBuy, "TEST", 10.0, 10.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.MarketSell, "TEST", 10.0, 10.0);

            string buyOrderId = broker.PlaceOrder(buyRequest).OrderId;
            Assert.IsNull(broker.GetPosition("TEST"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.BaseOnBar(t);
            Assert.IsNotNull(broker.GetPosition("TEST"));
            Assert.AreEqual(10.0, broker.GetPosition("TEST").Quantity);
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);

            // update the prices
            testBar = new Bar("TEST", DataInterval.Day, DateTime.Now, 11, 11, 11, 11, 100);
            t.Update(new Bar[] { testBar });

            string sellOrderId = broker.PlaceOrder(sellRequest).OrderId;
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.BaseOnBar(t);
            Assert.IsNull(broker.GetPosition("TEST"));
            Assert.AreEqual(initialFunds + 10, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);
            Assert.AreEqual(2, broker.GetAllOrders().Count);
        }

        [TestMethod]
        public void SellBuyProfitSucceeds()
        {
            MultiBar t = new MultiBar(new string[] { "TEST" });

            var testBar = new Bar("TEST", DataInterval.Day, DateTime.Now, 10, 10, 10, 10, 100);
            t.Update(new Bar[] { testBar });

            mockEngine.Setup(m => m.Bars).Returns(t);
            BackTestingBroker broker = new BackTestingBroker(initialFunds);
            broker.Initialize(mockEngine.Object);

            OrderRequest buyRequest = new OrderRequest(OrderType.MarketBuy, "TEST", 10.0, 10.0);
            OrderRequest sellRequest = new OrderRequest(OrderType.MarketSell, "TEST", 10.0, 10.0);

            string sellOrderId = broker.PlaceOrder(sellRequest).OrderId;
            Assert.IsNull(broker.GetPosition("TEST"));
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(sellOrderId).State);

            broker.BaseOnBar(t);
            Assert.IsNotNull(broker.GetPosition("TEST"));
            Assert.AreEqual(-10.0, broker.GetPosition("TEST").Quantity);
            Assert.AreEqual(initialFunds, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(sellOrderId).State);

            // update the prices
            testBar = new Bar("TEST", DataInterval.Day, DateTime.Now, 9, 9, 9, 9, 100);
            t.Update(new Bar[] { testBar });

            string buyOrderId = broker.PlaceOrder(buyRequest).OrderId;
            Assert.AreEqual(1, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Open, broker.GetOrder(buyOrderId).State);

            broker.BaseOnBar(t);
            Assert.IsNull(broker.GetPosition("TEST"));
            Assert.AreEqual(initialFunds + 10, broker.GetAccount().TotalValue);
            Assert.AreEqual(0, broker.GetOpenOrders().Count);
            Assert.AreEqual(OrderState.Filled, broker.GetOrder(buyOrderId).State);
            Assert.AreEqual(2, broker.GetAllOrders().Count);
        }
    }
}
