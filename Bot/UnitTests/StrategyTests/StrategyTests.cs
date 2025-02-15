using Bot.Models;
using Bot.Strategies;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Bot.Engine;
using Moq;

namespace StrategyTests
{
    [TestClass]
    public class StrategyTests
    {
        private IList<Bar> amcData;    

        [TestInitialize]
        public void Setup()
        {
            amcData = LoadData("./StrategyTests/AMC.csv", "AMC", DataInterval.Day);
        }

        /*
        [TestMethod]
        public void SMACrossoverStrategy_LongOnly()
        {
            var symbol = "AMC";
            Bars bars = new Bars(new string[] { symbol });
            var strat = new SMACrossoverStrategy();
            BackTestingBroker broker = new BackTestingBroker();

            Mock<ITradingEngine> engine = new Mock<ITradingEngine>();
            engine.Setup(m => m.Bars).Returns(bars);
            engine.Setup(m => m.Broker).Returns(broker);
            engine.Setup(m => m.Strategy).Returns(strat);

            broker.Initialize(engine.Object, new string[] { "1000" });
            strat.Initialize(
                engine.Object,
                new string[] { 
                    symbol, 
                    "5", 
                    "30", 
                    "true"
                });

            amcData = amcData.Select(x => x).OrderBy(x => x.DateTime).ToList();

            foreach (var bar in amcData)
            {
                bars.Update(new Bar[] { bar });
                broker.OnBar(bars);
                strat.OnBar(bars);
            }

            var expectedNumTrades = 10;

            Assert.AreEqual(expectedNumTrades, broker.OrderHistory.Count);

            List<Order> expectedOrders = new List<Order>()
            {
                new Order("order1", new DateTime(2020, 04, 22), new DateTime(2020, 04, 23), symbol, 3.2, 3.18, 10, OrderType.MarketBuy, OrderState.Filled),
                new Order("order2", new DateTime(2020, 06, 24), new DateTime(2020, 06, 25), symbol, 4.57, 4.79, 10, OrderType.MarketSell, OrderState.Filled),
                new Order("order3", new DateTime(2020, 08, 10), new DateTime(2020, 08, 11), symbol, 4.7, 4.47, 10, OrderType.MarketBuy, OrderState.Filled),
                new Order("order4", new DateTime(2020, 09, 18), new DateTime(2020, 09, 21), symbol, 5.42, 5.67, 10, OrderType.MarketSell, OrderState.Filled),
                new Order("order5", new DateTime(2020, 11, 13), new DateTime(2020, 11, 16), symbol, 3.39, 2.97, 10, OrderType.MarketBuy, OrderState.Filled),
                new Order("order6", new DateTime(2020, 11, 17), new DateTime(2020, 11, 18), symbol, 3.08, 2.98, 10, OrderType.MarketSell, OrderState.Filled),
                new Order("order7", new DateTime(2020, 11, 18), new DateTime(2020, 11, 19), symbol, 3.16, 3.26, 10, OrderType.MarketBuy, OrderState.Filled),
                new Order("order8", new DateTime(2020, 12, 16), new DateTime(2020, 12, 17), symbol, 2.8, 2.78, 10, OrderType.MarketSell, OrderState.Filled),
                new Order("order9", new DateTime(2021, 01, 21), new DateTime(2021, 01, 22), symbol, 2.91, 2.98, 10, OrderType.MarketBuy, OrderState.Filled),
                new Order("order10", new DateTime(2021, 02, 18), new DateTime(2021, 02, 19), symbol, 5.54, 5.51, 10, OrderType.MarketSell, OrderState.Filled),
            };

            var orderFound = false;
            foreach (var expectedOrder in expectedOrders)
            {
                orderFound = false;
                foreach (var actualOrder in broker.OrderHistory)
                {
                    if (!orderFound)
                    {
                        if (
                            actualOrder.FillPrice == expectedOrder.FillPrice &&
                            actualOrder.ExecutionTime == expectedOrder.ExecutionTime &&
                            actualOrder.TargetPrice == expectedOrder.TargetPrice &&
                            actualOrder.Type == expectedOrder.Type &&
                            actualOrder.Quantity == expectedOrder.Quantity)
                        {
                            orderFound = true;
                        }
                    }
                }
                Assert.IsTrue(orderFound);
            }
        }
        */


        public IList<Bar> LoadData(
           string fileName,
           string symbol,
           DataInterval interval)
        {
            Path.GetFullPath(fileName);
            List<Bar> barList = new List<Bar>();

            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // first line is headers
                parser.ReadFields();

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    DateTime dateTime = DateTime.ParseExact(
                        fields[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    double open = double.Parse(fields[1]);
                    double high = double.Parse(fields[2]);
                    double low = double.Parse(fields[3]);
                    double close = double.Parse(fields[4]);
                    double adjClose = double.Parse(fields[5]);
                    int volume = int.Parse(fields[6]);

                    Bar bar = new Bar(
                        symbol, interval, dateTime, open, high, low, close, volume);

                    barList.Add(bar);
                }
            }

            return barList;
        }
    }
}
