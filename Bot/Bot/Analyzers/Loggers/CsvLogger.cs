using Bot.Engine;
using Bot.Engine.Events;
using Bot.Indicators;
using Bot.Models;
using Bot.Models.Interfaces;
using CsvHelper;
using System;
using System.Globalization;
using System.IO;

namespace Bot.Analyzers.Loggers
{
    public class CsvLogger : IAnalyzer, ITickReceiver, ITerminateReceiver
    {
        private ITradingEngine engine;
        private FileStream fileStream;
        private StreamWriter streamWriter;
        private CsvWriter csv;

        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public void Initialize(ITradingEngine engine, string[] args)
        {
            this.engine = engine;

            string csvPath = Path.Combine(
                engine.OutputPath,
                $"{engine.Strategy.GetType()}.{DateTimeOffset.Now.ToUnixTimeSeconds()}.csv");

            fileStream = new FileStream(csvPath, FileMode.OpenOrCreate, FileAccess.Write);
            streamWriter = new StreamWriter(fileStream);
            csv = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);
            WriteHeaders();
        }

        /// <summary>
        /// Writes headers.
        /// </summary>
        private void WriteHeaders()
        {
            csv.WriteField("Date");

            foreach (Tick t in engine.Ticks.ToArray())
            {
                csv.WriteField(t.Symbol);
            }

            foreach (IIndicator ind in engine.Strategy.Indicators)
            {
                csv.WriteField(ind.GetType());
            }

            foreach (string header in engine.Strategy.GetCsvHeaders())
            {
                csv.WriteField(header);
            }

            csv.WriteField("Portfolio Value");
            csv.WriteField("Cash Value");
            csv.NextRecord();
        }

        /// <summary>
        /// Log at every tick.
        /// </summary>
        /// <param name="ticks"></param>
        public void OnTick(IMultiBar ticks)
        {
            csv.WriteField(ticks[0].DateTime.ToString("o"));

            foreach (Tick t in engine.Ticks.ToArray())
            {
                csv.WriteField(t.Close);
            }

            foreach (IIndicator ind in engine.Strategy.Indicators)
            {
                csv.WriteField(ind.Value);
            }

            foreach (string value in engine.Strategy.GetCsvValues())
            {
                csv.WriteField(value);
            }

            IAccount account = engine.Broker.GetAccount();

            csv.WriteField(account.TotalValue);
            csv.WriteField(account.Cash);
            csv.NextRecord();
        }

        /// <summary>
        /// Dispose of things.
        /// </summary>
        public void OnTerminate()
        {
            csv.Dispose();
            streamWriter.Dispose();
            fileStream.Dispose();
        }

    }
}
