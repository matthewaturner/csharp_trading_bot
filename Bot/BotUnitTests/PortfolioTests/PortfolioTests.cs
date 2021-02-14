using Bot.Brokerage;
using Bot.Trading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BotUnitTests.PortfolioTests
{
    [TestClass]
    public class PortfolioTests
    {
        [TestMethod]
        public void Portfolio_EnterPosition_SubtractsCash()
        {
            var expectedAvailableCash = 0;
            var portfolio = new Portfolio(1000);
            var trade = new Trade() 
            {
                Ticker = "GME",
                Price = 100,
                Units = 10,
                TradeType = TradeType.Buy
            };

            portfolio.EnterPosition(trade);

            Assert.AreEqual(expectedAvailableCash, portfolio.AvailableCash);
        }

        [TestMethod]
        public void Portfolio_EnterPosition_PositionExists()
        {
            var expectedPostion = new Position("GME", PositionType.StockLong, 10, 100);

            var portfolio = new Portfolio(1000);
            var trade = new Trade()
            {
                Ticker = "GME",
                Price = 100,
                Units = 10,
                TradeType = TradeType.Buy
            };

            portfolio.EnterPosition(trade);

            var actualPosition = portfolio.CurrentPositions["GME"];

            Assert.AreEqual(expectedPostion.EntryPrice, actualPosition.EntryPrice);
            Assert.AreEqual(expectedPostion.Name, actualPosition.Name);
            Assert.AreEqual(expectedPostion.Size, actualPosition.Size);
            Assert.AreEqual(expectedPostion._Type, actualPosition._Type);
        }

        [TestMethod]
        public void Portfolio_ExitPosition_PositionRemoved()
        {
            var expectedAvailableCash = 10000;
            var portfolio = new Portfolio(1000);
            var buy = new Trade()
            {
                Ticker = "GME",
                Price = 100,
                Units = 10,
                TradeType = TradeType.Buy
            };

            portfolio.EnterPosition(buy);

            var sell = new Trade()
            {
                Ticker = "GME",
                Price = 1000,
                Units = -10,
                TradeType = TradeType.Buy
            };

            portfolio.ExitPosition(sell);

            Assert.AreEqual(expectedAvailableCash, portfolio.AvailableCash);
        }

        [TestMethod]
        public void Portfolio_ExitPosition_Sell()
        {
            var portfolio = new Portfolio(1000);
            var buy = new Trade()
            {
                Ticker = "GME",
                Price = 100,
                Units = 10,
                TradeType = TradeType.Sell
            };

            portfolio.EnterPosition(buy);

            Assert.IsTrue(portfolio.CurrentPositions.Count == 1);

            var sell = new Trade()
            {
                Ticker = "GME",
                Price = 1000,
                Units = -10,
                TradeType = TradeType.Buy
            };

            portfolio.ExitPosition(sell);

            Assert.IsTrue(portfolio.CurrentPositions.Count == 0);
        }
    }
}
