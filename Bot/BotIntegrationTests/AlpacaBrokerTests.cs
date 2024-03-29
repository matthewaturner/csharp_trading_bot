using Bot.Brokers;
using Bot.Brokers.Alpaca;
using Bot.Engine;
using Bot.Models;
using Bot.Models.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BotIntegrationTests
{
    [TestClass]
    public class AlpacaBrokerTests
    {
        public Mock<ITradingEngine> engine;
        public AlpacaBroker alpaca;

        [TestInitialize]
        public void Setup()
        {
            var alpacaConfig = new AlpacaConfig
            { 
                PaperApiBaseUrl = "https://paper-api.alpaca.markets",
                ApiKeyId = "AlpacaApiKeyId",
                ApiKey = "AlpacaApiKeySecret"
            };

            engine = new Mock<ITradingEngine>();
            alpaca = new AlpacaBroker(alpacaConfig);

            alpaca.Initialize(engine.Object);
        }

        [TestMethod]
        public void GetAssetInformation()
        {
            IAssetInformation assetInfo = alpaca.GetAssetInformation("MSFT");
            Assert.IsNotNull(assetInfo);
            Assert.AreEqual("MSFT", assetInfo.Symbol);
            Assert.IsTrue(assetInfo.Marginable);
        }

        [TestMethod]
        public void GetAccountInformation()
        {
            IAccount account = alpaca.GetAccount();
            Assert.IsNotNull(account);
        }

        [TestMethod]
        public void GetPositions()
        {
            IList<IPosition> positions = alpaca.GetPositions();
            Assert.IsNotNull(positions);
        }

        [TestMethod]
        public void PlaceOrderThenCancel()
        {
            IOrderRequest request = new OrderRequest(OrderType.MarketBuy, "MSFT", 1, 15);
            string orderId = alpaca.PlaceOrder(request).OrderId;
            Assert.IsNotNull(orderId);

            IOrder placedOrder = alpaca.GetOrder(orderId);
            Assert.IsNotNull(placedOrder);
            Assert.AreEqual("MSFT", placedOrder.Symbol);

            alpaca.CancelOrder(orderId);
            IOrder cancelledOrder = alpaca.GetOrder(orderId);
            Assert.IsNotNull(cancelledOrder);
            Assert.AreEqual(OrderState.Cancelled, cancelledOrder.State);
        }

        [TestMethod]
        public void PlaceFractionalOrderThenCancel()
        {
            IOrderRequest request = new OrderRequest(OrderType.MarketBuy, "MSFT", 1.5555555, 15);
            string orderId = alpaca.PlaceOrder(request).OrderId;
            Assert.IsNotNull(orderId);

            IOrder placedOrder = alpaca.GetOrder(orderId);
            Assert.IsNotNull(placedOrder);
            Assert.AreEqual("MSFT", placedOrder.Symbol);

            alpaca.CancelOrder(orderId);
            IOrder cancelledOrder = alpaca.GetOrder(orderId);
            Assert.IsNotNull(cancelledOrder);
            Assert.AreEqual(OrderState.Cancelled, cancelledOrder.State);
        }


        [TestMethod]
        public void PlaceOrderThenQueryOrders()
        {
            string orderId1, orderId2, orderId3;
            orderId1 = orderId2 = orderId3 = null;

            try
            {

                IOrderRequest request1 = new OrderRequest(OrderType.MarketSell, "MSFT", 1, 0);
                IOrderRequest request2 = new OrderRequest(OrderType.MarketBuy, "GOOG", 1, 0);
                IOrderRequest request3 = new OrderRequest(OrderType.MarketSell, "TSLA", 1, 0);

                orderId1 = alpaca.PlaceOrder(request1).OrderId;

                Thread.Sleep(1000);
                var queryTime = DateTime.UtcNow;

                orderId2 = alpaca.PlaceOrder(request2).OrderId;
                orderId3 = alpaca.PlaceOrder(request3).OrderId;

                IList<IOrder> orders = alpaca.QueryOrders(null, OrderState.Open, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
                Assert.IsNotNull(orders);
                Assert.AreEqual(3, orders.Count);

                orders = alpaca.QueryOrders(new string[] { "MSFT" }, OrderState.Open, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
                Assert.IsNotNull(orders);
                Assert.AreEqual(1, orders.Count);

                orders = alpaca.QueryOrders(null, OrderState.Open, queryTime, DateTime.UtcNow);
                Assert.IsNotNull(orders);
                Assert.AreEqual(2, orders.Count);

                orders = alpaca.QueryOrders(new string[] { "GOOG", "TSLA" }, OrderState.Open, DateTime.UtcNow.AddHours(-1), DateTime.UtcNow);
                Assert.IsNotNull(orders);
                Assert.AreEqual(2, orders.Count);
            }
            finally
            {
                if (orderId1 != null)
                {
                    alpaca.CancelOrder(orderId1);
                }
                if (orderId2 != null)
                {
                    alpaca.CancelOrder(orderId2);
                }
                if (orderId3 != null)
                {
                    alpaca.CancelOrder(orderId3);
                }
            }
        }
    }
}
