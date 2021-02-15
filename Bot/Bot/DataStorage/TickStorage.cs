using Core;
using Bot.Configuration;
using Bot.DataStorage.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Bot.DataCollection;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Bot.DataStorage
{
    public class TickStorage : ITickStorage
    {
        private SqlConfiguration sqlConfig;
        private TickContext context;
        private IDataSource dataSource;
        private readonly string connectionString;

        public TickStorage(
            IOptions<SqlConfiguration> sqlConfig,
            IKeyVaultManager kvManager,
            IDataSource dataSource)
        {
            this.dataSource = dataSource;
            this.sqlConfig = sqlConfig.Value;

            string connectionString = kvManager.GetSecretAsync(this.sqlConfig.ConnectionStringSecretName).Result;
            context = new TickContext(connectionString);
            context.Database.CreateIfNotExists();
            context.Configuration.AutoDetectChangesEnabled = false;
        }

        /// <summary>
        /// Get ticks for a symbol for some interval and over a range of time.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval">Tick interval.</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<IList<Tick>> GetTicksAsync(
            string symbol,
            TickInterval interval,
            DateTime start,
            DateTime end)
        {
            start = start.GetNextTradingDayInclusive();
            end = end.GetNextTradingDayInclusive();
            DateTime tradeEndInclusive = end.GetPreviousTradingDay();

            var query = context.Ticks
                .Where(t =>
                    t.Symbol == symbol
                    && t.TickInterval == interval
                    && t.DateTime >= start
                    && t.DateTime <= end)
                .OrderBy(t => t.DateTime);

            IList<Tick> sqlTicks = query.ToList();
            DateTime ticksStart, ticksEnd;

            // if no data was gathered from sql
            if (sqlTicks == null || !sqlTicks.Any())
            {
                Console.WriteLine($"Data not found in database. Fetching from source.");
                IList<Tick> sourceTicks = await dataSource.GetTicksAsync(symbol, interval, start, end);

                if (!sourceTicks.Any())
                {
                    Console.Error.WriteLine($"Failed to fetch any data from source.");
                    throw new Exception();
                }

                BulkInsertTicks(sourceTicks);

                DateTime sourceTicksStart = sourceTicks.FirstOrDefault().DateTime;
                DateTime sourceTicksEnd = sourceTicks.LastOrDefault().DateTime;

                if (Helpers.Compare(sourceTicksStart, start, interval) != 0 
                    || Helpers.Compare(sourceTicksEnd, tradeEndInclusive, interval) != 0)
                {
                    Console.Error.WriteLine($"Data from source only spans {sourceTicksStart.StandardToString()} to {sourceTicksStart.StandardToString()}");
                    throw new ArgumentOutOfRangeException();
                }

                return sourceTicks;
            }

            // if data was gathered from sql
            ticksStart = sqlTicks.FirstOrDefault().DateTime;
            ticksEnd = sqlTicks.LastOrDefault().DateTime;

            if (Helpers.Compare(ticksStart, start, interval) != 0 
                || Helpers.Compare(ticksEnd, tradeEndInclusive, interval) != 0)
            {
                Console.WriteLine($"Data from sql was insufficient date span. Fetching from source.");
                IList<Tick> sourceTicks = await dataSource.GetTicksAsync(symbol, interval, start, end);

                if (!sqlTicks.Any())
                {
                    Console.Error.WriteLine($"Failed to fetch any data from source.");
                    throw new Exception();
                }
                string testPrimaryKey = sourceTicks.FirstOrDefault().PrimaryKey();

                HashSet<string> sqlTickSet = sqlTicks.Select(t => t.PrimaryKey()).ToHashSet();
                BulkInsertTicks(sourceTicks.Where(t => !sqlTickSet.Contains(t.PrimaryKey())));

                DateTime sourceTicksStart = sourceTicks.FirstOrDefault().DateTime;
                DateTime sourceTicksEnd = sourceTicks.LastOrDefault().DateTime;

                if (Helpers.Compare(sourceTicksStart, start, interval) != 0 || Helpers.Compare(sourceTicksEnd, end, interval) != 0)
                {
                    Console.Error.WriteLine($"Data from source only spans {sourceTicksStart.StandardToString()} to {sourceTicksEnd.StandardToString()}");
                    throw new ArgumentOutOfRangeException();
                }

                return sourceTicks;
            }

            return sqlTicks;
        }

        private void BulkInsertTicks(IEnumerable<Tick> ticks)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            SqlBulkCopy bulkCopy = new SqlBulkCopy(connection);

            DataTable data = new DataTable();
            data.Columns.Add(new DataColumn("Symbol", typeof(string)));
            data.Columns.Add(new DataColumn("DateTime", typeof(DateTime)));
            data.Columns.Add(new DataColumn("TickInterval", typeof(int)));
            data.Columns.Add(new DataColumn("Open", typeof(double)));
            data.Columns.Add(new DataColumn("High", typeof(double)));
            data.Columns.Add(new DataColumn("Low", typeof(double)));
            data.Columns.Add(new DataColumn("Close", typeof(double)));
            data.Columns.Add(new DataColumn("AdjClose", typeof(double)));
            data.Columns.Add(new DataColumn("Volume", typeof(double)));

            foreach(Tick t in ticks)
            {
                DataRow row = data.NewRow();
                row["Symbol"] = t.Symbol;
                row["DateTime"] = t.DateTime;
                row["TickInterval"] = t.TickInterval;
                row["Open"] = t.Open;
                row["High"] = t.High;
                row["Low"] = t.Low;
                row["Close"] = t.Close;
                row["AdjClose"] = t.AdjClose;
                row["Volume"] = t.Volume;
                data.Rows.Add(row);
            }

            bulkCopy.DestinationTableName = "Ticks";
            bulkCopy.ColumnMappings.Add("Symbol", "Symbol");
            bulkCopy.ColumnMappings.Add("DateTime", "DateTime");
            bulkCopy.ColumnMappings.Add("TickInterval", "TickInterval");
            bulkCopy.ColumnMappings.Add("Open", "Open");
            bulkCopy.ColumnMappings.Add("High", "High");
            bulkCopy.ColumnMappings.Add("Low", "Low");
            bulkCopy.ColumnMappings.Add("Close", "Close");
            bulkCopy.ColumnMappings.Add("AdjClose", "AdjClose");
            bulkCopy.ColumnMappings.Add("Volume", "Volume");

            connection.Open();
            bulkCopy.WriteToServer(data);
            connection.Close();
        }
    }
}
