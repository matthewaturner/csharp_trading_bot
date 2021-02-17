using Bot.Models;
using Bot.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace PortfolioTests
{
    [TestClass]
    public class PortfolioTests
    {
        /*
        [TestMethod]
        public void EnterPositionSucceeds()
        {


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
