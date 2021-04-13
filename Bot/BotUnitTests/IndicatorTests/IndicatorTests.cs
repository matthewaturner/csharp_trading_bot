using Bot.Indicators;
using Bot.Models;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Bot;
using Moq;

namespace IndicatorTests
{
    [TestClass]
    public class IndicatorTests
    {
        private IList<IMultiTick> msftData;
        private IDictionary<string, IList<double>> msftResults;

        [TestInitialize]
        public void Setup()
        {
            msftData = LoadData("./IndicatorTests/MSFT.csv", "MSFT", TickInterval.Day);
            msftResults = LoadResults("./IndicatorTests/MSFT.csv");
        }

        [TestMethod]
        public void SMA_Hydration()
        {
            IIndicator sma = new SimpleMovingAverage(30, (IMultiTick t) => t["MSFT"].AdjClose);
            Assert.IsFalse(sma.Hydrated);

            sma = new SimpleMovingAverage(30, (IMultiTick t) => t["MSFT"].AdjClose);
            ReplayData(sma, msftData.Take(29).ToList());
            Assert.IsFalse(sma.Hydrated);

            Assert.AreEqual(double.NaN, sma.Value);

            sma = new SimpleMovingAverage(30, (IMultiTick t) => t["MSFT"].AdjClose);
            ReplayData(sma, msftData.Take(30).ToList());
            Assert.IsTrue(sma.Hydrated);

            sma = new SimpleMovingAverage(30, (IMultiTick t) => t["MSFT"].AdjClose);
            ReplayData(sma, msftData.Take(60).ToList());
            Assert.IsTrue(sma.Hydrated);

            sma = new SimpleMovingAverage(30, (IMultiTick t) => t["MSFT"].AdjClose);
            ReplayData(sma, msftData.Take(61).ToList());
            Assert.IsTrue(sma.Hydrated);

            sma = new SimpleMovingAverage(30, (IMultiTick t) => t["MSFT"].AdjClose);
            ReplayData(sma, msftData.Take(62).ToList());
            Assert.IsTrue(sma.Hydrated);
        }

        [TestMethod]
        public void SMA_Values()
        {
            IIndicator sma = new SimpleMovingAverage(30, (IMultiTick t) => t["MSFT"].AdjClose);
            IList<double> smaResults = msftResults[nameof(SimpleMovingAverage)]
                .Select(obj => (double)obj)
                .ToList();

            ReplayData(sma, msftData.Take(29).ToList());
            ReplayAndCompare(
                sma, 
                msftData.Skip(29).ToList(), 
                smaResults.Skip(29).ToList());
        }

        [TestMethod]
        public void MAC_Hydration()
        {
            IIndicator mac = new MovingAverageCrossover(16, 64, (IMultiTick t) => t["MSFT"].AdjClose);
            Assert.IsFalse(mac.Hydrated);

            ReplayData(mac, msftData.Take(15).ToList());
            Assert.IsFalse(mac.Hydrated);

            ReplayData(mac, msftData.Skip(15).Take(48).ToList());
            Assert.IsFalse(mac.Hydrated);

            Assert.AreEqual(double.NaN, mac.Value);

            ReplayData(mac, msftData.Skip(63).Take(1).ToList());
            Assert.IsTrue(mac.Hydrated);
        }

        [TestMethod]
        public void MAC_Values()
        {
            IIndicator mac = new MovingAverageCrossover(10, 30, (IMultiTick t) => t["MSFT"].AdjClose);
            IList<double> macResults = msftResults[nameof(MovingAverageCrossover)]
                .Select(obj => (double)obj)
                .ToList();

            ReplayData(mac, msftData.Take(29).ToList());
            ReplayAndCompare(
                mac,
                msftData.Skip(29).ToList(),
                macResults.Skip(29).ToList());
        }

        [TestMethod]
        public void MovStdDev_Hydration()
        {
            IIndicator stdDev = new MovingStandardDeviation(30, (IMultiTick t) => t["MSFT"].AdjClose);
            Assert.IsFalse(stdDev.Hydrated);

            ReplayData(stdDev, msftData.Take(15).ToList());
            Assert.IsFalse(stdDev.Hydrated);

            ReplayData(stdDev, msftData.Skip(15).Take(14).ToList());
            Assert.IsFalse(stdDev.Hydrated);

            Assert.AreEqual(double.NaN, stdDev.Value);

            ReplayData(stdDev, msftData.Skip(29).Take(1).ToList());
            Assert.IsTrue(stdDev.Hydrated);
        }

        [TestMethod]
        public void MovStdDev_Values()
        {
            IIndicator stdDev = new MovingStandardDeviation(10, (IMultiTick t) => t["MSFT"].AdjClose);
            IList<double> stdDevResults = msftResults[nameof(MovingStandardDeviation)]
                .Select(obj => (double)obj)
                .ToList();

            ReplayData(stdDev, msftData.Take(9).ToList());
            ReplayAndCompare(
                stdDev,
                msftData.Skip(9).ToList(),
                stdDevResults.Skip(9).ToList());
        }

        [TestMethod]
        public void Bollinger_Hydration()
        {
            IIndicator boll = new BollingerBand(10, 1, .05, (IMultiTick t) => t["MSFT"].AdjClose);
            IList<int> bollResults = msftResults[nameof(BollingerBand)]
                .Select(obj => (int)obj)
                .ToList();

            ReplayData(boll, msftData.Take(4).ToList());
            Assert.IsFalse(boll.Hydrated);

            ReplayData(boll, msftData.Skip(4).Take(5).ToList());
            Assert.IsFalse(boll.Hydrated);

            Assert.AreEqual(double.NaN, boll.Value);

            ReplayData(boll, msftData.Skip(9).Take(1).ToList());
            Assert.IsTrue(boll.Hydrated);
        }

        [TestMethod]
        public void Bollinger_Values()
        {
            IIndicator boll = new BollingerBand(10, 1, .05, (IMultiTick t) => t["MSFT"].AdjClose);
            IList<double> bollResults = msftResults[nameof(BollingerBand)]
                .Select(obj => (double)obj)
                .ToList();

            ReplayData(boll, msftData.Take(9).ToList());
            ReplayAndCompare(
                boll,
                msftData.Skip(9).ToList(),
                bollResults.Skip(9).ToList());
        }

        public void ReplayAndCompare(
            IIndicator indicator, 
            IList<IMultiTick> data, 
            IList<double> results)
        {
            IList<double> values = new List<double>();
            for (int i=0; i<data.Count; i++)
            {
                indicator.OnTick(data[i]);
                values.Add(indicator.Value);
                if (Helpers.CompareDoubles(indicator.Value, results[i]) != 0)
                {
                    Assert.Fail($"Values {indicator.Value} and {results[i]} " +
                        $"are unequal at line {i}.");
                }
            }
        }

        public void ReplayData(IIndicator indicator, IList<IMultiTick> data)
        {
            foreach (IMultiTick t in data)
            {
                indicator.OnTick(t);
            }
        }

        public Dictionary<string, IList<double>> LoadResults(string fileName)
        {
            Path.GetFullPath(fileName);

            Dictionary<string, IList<double>> results = new Dictionary<string, IList<double>>();
            List<double> sma30Results = new List<double>();
            List<double> mac10_30Results = new List<double>();
            List<double> stdDevResults = new List<double>();
            List<double> bollResults = new List<double>();

            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // first line is headers
                parser.ReadFields();

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    double sma30 = string.IsNullOrWhiteSpace(fields[7]) ?
                        double.NaN : double.Parse(fields[7]);
                    double mac10_30 = string.IsNullOrWhiteSpace(fields[9]) ?
                        -2 : int.Parse(fields[9]);
                    double stdDev = string.IsNullOrWhiteSpace(fields[10]) ?
                        double.NaN : double.Parse(fields[10]);
                    double boll = string.IsNullOrWhiteSpace(fields[15]) ?
                        -2 : int.Parse(fields[15]);

                    sma30Results.Add(sma30);
                    mac10_30Results.Add(mac10_30);
                    stdDevResults.Add(stdDev);
                    bollResults.Add(boll);
                }
            }

            results.Add(nameof(SimpleMovingAverage), sma30Results);
            results.Add(nameof(MovingAverageCrossover), mac10_30Results);
            results.Add(nameof(MovingStandardDeviation), stdDevResults);
            results.Add(nameof(BollingerBand), bollResults);
            return results;
        }

        public IList<IMultiTick> LoadData(
            string fileName, 
            string symbol, 
            TickInterval interval)
        {
            Path.GetFullPath(fileName);
            List<IMultiTick> tickList = new List<IMultiTick>();

            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // first line is headers
                parser.ReadFields();

                while(!parser.EndOfData)
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

                    Tick tick = new Tick(
                        symbol, interval, dateTime, open, high, low, close, adjClose, volume);

                    Mock<IMultiTick> mockTicks = new Mock<IMultiTick>();
                    mockTicks.Setup(m => m[It.IsAny<string>()]).Returns(tick);

                    tickList.Add(mockTicks.Object);
                }
            }

            return tickList;
        }
    }
}
