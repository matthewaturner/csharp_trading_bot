using Bot.DataStorage.Models;
using Bot.Indicators;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

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
        public void SMA_NotHydrated()
        {
            IIndicator<double?> sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            Assert.IsFalse(sma.IsHydrated());
            Assert.IsNull(sma.Value);

            sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            ReplayData(sma, msftData.Take(29).ToList());
            Assert.IsFalse(sma.IsHydrated());
            Assert.IsNull(sma.Value);
        }

        [TestMethod]
        public void SMA_Hydrated()
        {
            IIndicator<double?> sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            ReplayData(sma, msftData.Take(30).ToList());
            Assert.IsTrue(sma.IsHydrated());
            Assert.IsNotNull(sma.Value);

            sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            ReplayData(sma, msftData.Take(60).ToList());
            Assert.IsTrue(sma.IsHydrated());
            Assert.IsNotNull(sma.Value);

            sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            ReplayData(sma, msftData.Take(61).ToList());
            Assert.IsTrue(sma.IsHydrated());
            Assert.IsNotNull(sma.Value);
        }

        [TestMethod]
        public void SMA_Values()
        {
            IIndicator<double?> sma = new SimpleMovingAverage(30, (Tick t) => t.AdjClose);
            IList<double?> smaResults = msftResults[nameof(SimpleMovingAverage)]
                .Select(obj => (double?)obj)
                .ToList();

            ReplayData(sma, msftData.Take(29).ToList());
            ReplayAndCompare(
                sma, msftData.Skip(29).ToList(), smaResults.Skip(29).ToList(), CompareDoubles);
        }

        public bool CompareDoubles(double? a, double? b)
        {
            if (a == null || b == null)
            {
                Assert.AreEqual(a, b);
                return a == b;
            }

            double diff = a.Value - b.Value;
            bool areEqual = Math.Abs(a.Value - b.Value) < .000001;

            return Math.Abs(a.Value - b.Value) < .000001;
        }

        public void ReplayAndCompare<T>(
            IIndicator<T> indicator, 
            IList<Tick> data, 
            IList<T> results, 
            Func<T, T, bool> compareFunc)
        {
            IList<T> values = new List<T>();
            for (int i=0; i<data.Count; i++)
            {
                indicator.OnTick(data[i]);
                values.Add(indicator.Value);
                if (!compareFunc(indicator.Value, results[i]))
                {
                    Assert.Fail($"Values {indicator.Value} and {results[i]} " +
                        $"are unequal at line {i}.");
                }
            }
        }

        public IList<T> ReplayData<T>(IIndicator<T> indicator, IList<Tick> data)
        {
            IList<T> values = new List<T>();
            foreach (Tick t in data)
            {
                indicator.OnTick(t);
                values.Add(indicator.Value);
            }

            return values;
        }

        public Dictionary<string, IList<object>> LoadResults(string fileName)
        {
            Path.GetFullPath(fileName);

            Dictionary<string, IList<object>> results = new Dictionary<string, IList<object>>();
            List<object> smaResults = new List<object>();

            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // first line is headers
                parser.ReadFields();

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    double? movAvg = string.IsNullOrWhiteSpace(fields[7]) ?
                        null : (double?)double.Parse(fields[7]);

                    smaResults.Add(movAvg);
                }
            }

            results.Add(nameof(SimpleMovingAverage), smaResults);
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
