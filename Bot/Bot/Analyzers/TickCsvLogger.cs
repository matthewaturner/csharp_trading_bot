
using Bot.Engine;
using Bot.Engine.Events;
using Bot.Indicators;
using Bot.Models;
using CsvHelper;
using System;
using System.Globalization;
using System.IO;

namespace Bot.Analyzers
{
    public class TickCsvLogger : IAnalyzer, ITickReceiver, ITerminateReceiver
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
            string outputPath = args[0];
            this.engine = engine;

            string csvPath = Path.Combine(outputPath, $"{engine.Strategy.GetType()}.{DateTimeOffset.Now.ToUnixTimeSeconds()}.csv");
            Directory.CreateDirectory(Path.GetDirectoryName(csvPath));
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
        public void OnTick(ITicks ticks)
        {
            csv.WriteField(ticks[0].DateTime.ToString("o"));

            foreach (Tick t in engine.Ticks.ToArray())
            {
                csv.WriteField(t.AdjClose);
            }

            foreach (IIndicator ind in engine.Strategy.Indicators)
            {
                csv.WriteField(ind.Value);
            }

            foreach (string value in engine.Strategy.GetCsvValues())
            {
                csv.WriteField(value);
            }

            csv.WriteField(engine.Broker.PortfolioValue());
            csv.WriteField(engine.Broker.CashValue());
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
