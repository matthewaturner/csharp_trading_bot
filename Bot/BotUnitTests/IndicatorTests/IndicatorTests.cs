using Bot.DataStorage.Models;
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
using Bot.Exceptions;

namespace IndicatorTests
{
    [TestClass]
    public class IndicatorTests
    {
        private IList<Tick> msftData;
        private IDictionary<string, IList<object>> msftResults;

        [TestInitialize]
        public void Setup()
        {
            msftData = LoadData("./IndicatorTests/MSFT.csv", "MSFT", TickInterval.Day);
            msftResults = LoadResults("./IndicatorTests/MSFT.csv");
        }

        [TestMethod]
        public void SMA_Hydration()
        {
            IIndicator<double> sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            Assert.IsFalse(sma.Hydrated);

            sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            ReplayData(sma, msftData.Take(29).ToList());
            Assert.IsFalse(sma.Hydrated);

            Assert.ThrowsException<IndicatorNotHydratedException>(() => sma.Value);

            sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            ReplayData(sma, msftData.Take(30).ToList());
            Assert.IsTrue(sma.Hydrated);

            sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            ReplayData(sma, msftData.Take(60).ToList());
            Assert.IsTrue(sma.Hydrated);

            sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            ReplayData(sma, msftData.Take(61).ToList());
            Assert.IsTrue(sma.Hydrated);

            sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            ReplayData(sma, msftData.Take(62).ToList());
            Assert.IsTrue(sma.Hydrated);
        }

        [TestMethod]
        public void SMA_Values()
        {
            IIndicator<double> sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            IList<double> smaResults = msftResults[nameof(SimpleMovingAverage)]
                .Select(obj => (double)obj)
                .ToList();

            ReplayData(sma, msftData.Take(29).ToList());
            ReplayAndCompare(sma, msftData.Skip(29).ToList(), smaResults.Skip(29).ToList(), Helpers.CompareDoubles);
        }

        [TestMethod]
        public void MAC_Hydration()
        {
            IIndicator<int> mac = new MovingAverageCrossover(16, 64, (Tick t) => t.AdjClose);
            Assert.IsFalse(mac.Hydrated);

            ReplayData(mac, msftData.Take(15).ToList());
            Assert.IsFalse(mac.Hydrated);

            ReplayData(mac, msftData.Skip(15).Take(48).ToList());
            Assert.IsFalse(mac.Hydrated);

            Assert.ThrowsException<IndicatorNotHydratedException>(() => mac.Value);

            ReplayData(mac, msftData.Skip(63).Take(1).ToList());
            Assert.IsTrue(mac.Hydrated);
        }

        [TestMethod]
        public void MAC_Values()
        {
            IIndicator<int> mac = new MovingAverageCrossover(10, 30, (Tick t) => t.AdjClose);
            IList<int> macResults = msftResults[nameof(MovingAverageCrossover)]
                .Select(obj => (int)obj)
                .ToList();

            ReplayData(mac, msftData.Take(29).ToList());
            ReplayAndCompare(
                mac, 
                msftData.Skip(29).ToList(), 
                macResults.Skip(29).ToList(), 
                (int a, int b) => { return a == b ? 0 : -1; });
        }

        public void ReplayAndCompare<T>(
            IIndicator<T> indicator, 
            IList<Tick> data, 
            IList<T> results, 
            Func<T, T, int> compareFunc)
        {
            IList<T> values = new List<T>();
            for (int i=0; i<data.Count; i++)
            {
                indicator.OnTick(data[i]);
                values.Add(indicator.Value);
                if (compareFunc(indicator.Value, results[i]) != 0)
                {
                    Assert.Fail($"Values {indicator.Value} and {results[i]} " +
                        $"are unequal at line {i}.");
                }
            }
        }

        public void ReplayData<T>(IIndicator<T> indicator, IList<Tick> data)
        {
            foreach (Tick t in data)
            {
                indicator.OnTick(t);
            }
        }

        public Dictionary<string, IList<object>> LoadResults(string fileName)
        {
            Path.GetFullPath(fileName);

            Dictionary<string, IList<object>> results = new Dictionary<string, IList<object>>();
            List<object> sma30Results = new List<object>();
            List<object> mac10_30Results = new List<object>();

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
                    int mac10_30 = string.IsNullOrWhiteSpace(fields[9]) ?
                        -2 : int.Parse(fields[9]);

                    sma30Results.Add(sma30);
                    mac10_30Results.Add(mac10_30);
                }
            }

            results.Add(nameof(SimpleMovingAverage), sma30Results);
            results.Add(nameof(MovingAverageCrossover), mac10_30Results);
            return results;
        }

        public IList<Tick> LoadData(
            string fileName, 
            string symbol, 
            TickInterval interval)
        {
            Path.GetFullPath(fileName);
            List<Tick> tickList = new List<Tick>();

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
                    double volume = double.Parse(fields[6]);

                    Tick tick = new Tick(
                        symbol, interval, dateTime, open, high, low, close, adjClose, volume);

                    tickList.Add(tick);
                }
            }

            return tickList;
        }
    }
}
