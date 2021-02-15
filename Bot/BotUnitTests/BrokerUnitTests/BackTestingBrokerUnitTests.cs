using Bot.Brokerage;
using Bot.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace BrokerUnitTests
{
    [TestClass]
    public class BackTestingBrokerUnitTests
    {
        [TestMethod]
        public void ExecuteTrade_Buy_ThrowException()
        {
            var portfolio = new Portfolio(1000);
            var broker = new BackTestingBroker(portfolio);

            var trade = new Trade()
            {
                Ticker = "GME",
                Price = 420.69,
                Units = 10,
                TradeType = TradeType.Buy
            };

            Assert.ThrowsException<Exception>(() => broker.ExecuteTrade(trade));
        }

        [TestMethod]
        public void BulkExecuteTrade_Buy_ThrowException()
        {
            var portfolio = new Portfolio(1000);
            var broker = new BackTestingBroker(portfolio);

            var trades = new List<Trade>()
            {
                new Trade(){Ticker = "GME", Price = 90, Units = 10, TradeType = TradeType.Buy },
                new Trade(){Ticker = "AMC", Price = 10, Units = 20, TradeType = TradeType.Buy }
            };

            Assert.ThrowsException<Exception>(() => broker.BulkExecuteTrade(trades));
        }

        [TestMethod]
        public void ExecuteTrade_Buy()
        {
            var startingCash = 10000;
            var portfolio = new Portfolio(startingCash);
            var broker = new BackTestingBroker(portfolio);

            var trade = new Trade()
            {
                Ticker = "GME",
                Price = 100,
                Units = 10,
                TradeType = TradeType.Buy
            };

            broker.ExecuteTrade(trade);
            var expectedAvailableCash = startingCash - trade.GetTradeValue();
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

            var trades = new List<Trade>()
            {
                new Trade(){Ticker = "GME", Price = 100, Units = 10, TradeType = TradeType.Buy },
                new Trade(){Ticker = "AMC", Price = 10, Units = 20, TradeType = TradeType.Buy }
            };

            broker.BulkExecuteTrade(trades);
            var expectedAvailableCash = startingCash - trades[0].GetTradeValue() - trades[1].GetTradeValue();

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

            var buy = new Trade()
            {
                Ticker = "GME",
                Price = 100,
                Units = 10,
                TradeType = TradeType.Buy
            };

            broker.ExecuteTrade(buy);

            var sell = new Trade() 
            {
                Ticker = "GME",
                Price = 1000,
                Units = -10,
                TradeType = TradeType.Sell
            };

            broker.ExecuteTrade(sell);
            var expectedAvailableCash = sell.GetTradeValue();

            Assert.AreEqual(expectedAvailableCash, broker.Portfolio.AvailableCash);
        }
    }
}
