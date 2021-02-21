using Bot;
using Bot.Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace PortfolioTests
{
    [TestClass]
    public class PortfolioTests
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
        public void GetTotalValue()
        {
            var msftPos = new Position("MSFT", 10.0);
            var gmePos = new Position("GME", -5.0);
            var amcPos = new Position("AMC", 12.0);
            double cashBalance = 100.0;

            var portfolio = new Portfolio(1000);
            portfolio.Buy("GME", 10, 49.51);

            Assert.AreEqual(504.9, portfolio.CashBalance.Round());
            Assert.AreEqual(1, portfolio.Positions.Count);
            Assert.AreEqual(1000.0, portfolio.CurrentValue(ticks, (t) => t.AdjClose));
        }

        [TestMethod]
        public void EnterExitPositionRemoved()
        {
            var portfolio = new Portfolio(1000);

            portfolio.Buy("GME", 10, 100.0);
            Assert.AreEqual(0, portfolio.CashBalance);
            Assert.AreEqual(1, portfolio.Positions.Count);

            portfolio.Sell("GME", 10, 90.0);
            Assert.AreEqual(900.0, portfolio.CashBalance);
            Assert.AreEqual(0, portfolio.Positions.Count);
        }

        [TestMethod]
        public void ShortSaleExitPositionRemoved()
        {
            var portfolio = new Portfolio(1000);

            portfolio.Sell("GME", 10, 100.0);
            Assert.AreEqual(2000.0, portfolio.CashBalance);
            Assert.AreEqual(portfolio.Positions.Count, 1);

            portfolio.Buy("GME", 10, 90.0);
            Assert.AreEqual(1100.0, portfolio.CashBalance);
            Assert.AreEqual(0, portfolio.Positions.Count);
        }
    }
}
