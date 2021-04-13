using Bot.Brokers;
using Bot.Configuration;
using Bot.Engine;
using Bot.Models;
using Bot.Models.Interfaces;
using Core.Azure;
using Core.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

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
            var kvConfig = new KeyVaultConfiguration
            {
                BaseUrl = "https://tradebotvault.vault.azure.net/"
            };

            var alpacaConfig = new AlpacaConfiguration
            { 
                PaperApiBaseUrl = "https://paper-api.alpaca.markets",
                PaperApiKeyIdSecretName = "AlpacaApiKeyId",
                PaperApiKeySecretName = "AlpacaApiKeySecret"
            };

            Mock<IOptionsSnapshot<KeyVaultConfiguration>> kvConfigSnapshot = new Mock<IOptionsSnapshot<KeyVaultConfiguration>>();
            kvConfigSnapshot.Setup(m => m.Value).Returns(kvConfig);

            Mock<IOptionsSnapshot<AlpacaConfiguration>> alpacaConfigSnapshot = new Mock<IOptionsSnapshot<AlpacaConfiguration>>();
            alpacaConfigSnapshot.Setup(m => m.Value).Returns(alpacaConfig);

            engine = new Mock<ITradingEngine>();

            KeyVaultManager keyVaultManager = new KeyVaultManager(kvConfigSnapshot.Object);
            alpaca = new AlpacaBroker(alpacaConfigSnapshot.Object, keyVaultManager);

            alpaca.Initialize(engine.Object, new string[] { "true" });
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
            string orderId = alpaca.PlaceOrder(request);
            Assert.IsNotNull(orderId);

            IOrder placedOrder = alpaca.GetOrder(orderId);
            Assert.IsNotNull(placedOrder);
            Assert.AreEqual("MSFT", placedOrder.Symbol);

            alpaca.CancelOrder(orderId);
            IOrder cancelledOrder = alpaca.GetOrder(orderId);
            Assert.IsNotNull(cancelledOrder);
            Assert.AreEqual(OrderState.Cancelled, cancelledOrder.State);
        }
    }
}
