using Bot.Models;
using Bot.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;

namespace PortfolioTests
{
    [TestClass]
    public class PortfolioTests
    {
        CurrentTicks currentTicks;

        public void Setup()
        {
            currentTicks = new CurrentTicks(new string[] { "gme", "msft", "amc" });

            var msftTick = new Tick("msft", TickInterval.Day, DateTime.Now, 245.03, 246.13, 242.92, 243.70, 243.70, 26708200);
            var gmeTick = new Tick("gme", TickInterval.Day, DateTime.Now, 52.22, 53.50, 49.04, 49.51, 49.51, 8140700);
            var amcTick = new Tick("amc", TickInterval.Day, DateTime.Now, 6.03, 6.05, 5.49, 5.65, 5.65, 60690200);
        }

        /*
        [TestMethod]
        public void GetTotalValue()
        {
            var msftPos = new Position("msft", 10.0);
            var gmePos = new Position("gme", -5.0);
            var amcPos = new Position("amc", 12.0);
            double cashBalance = 100.0;

            var portfolio = new Portfolio(1000);
            portfolio.("GME", 10, 100.0);

            Assert.AreEqual(0, portfolio.CashBalance);
            Assert.AreEqual(1, portfolio.Positions.Count);
            Assert.AreEqual(1100.0, portfolio.GetTotalValue(currentPrices));
        }

        [TestMethod]
        public void EnterPositionInsufficientCash()
        {
            Portfolio portfolio = new Portfolio(100);
            Assert.ThrowsException<InvalidOrderException>(
                () => portfolio.BuySymbol("GME", 10, 100.0));
        }

        [TestMethod]
        public void EnterExitPositionRemoved()
        {
            var portfolio = new Portfolio(1000);

            portfolio.BuySymbol("GME", 10, 100.0);
            Assert.AreEqual(0, portfolio.CashBalance);
            Assert.AreEqual(1, portfolio.Positions.Count);

            portfolio.SellSymbol("GME", 10, 90.0);
            Assert.AreEqual(900.0, portfolio.CashBalance);
            Assert.AreEqual(0, portfolio.Positions.Count);
        }

        [TestMethod]
        public void ShortSaleExitPositionRemoved()
        {
            var portfolio = new Portfolio(1000);

            portfolio.SellSymbol("GME", 10, 100.0);
            Assert.AreEqual(2000.0, portfolio.CashBalance);
            Assert.AreEqual(portfolio.Positions.Count, 1);

            portfolio.BuySymbol("GME", 10, 90.0);
            Assert.AreEqual(1100.0, portfolio.CashBalance);
            Assert.AreEqual(0, portfolio.Positions.Count);
        }
        */
    }
}
