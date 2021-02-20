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
using Bot.Models;

namespace Bot.DataStorage
{
    public class TickStorage : ITickStorage
    {        
        private SqlConfiguration sqlConfig;
        private TickContext context;
        private IDataSource dataSource;
        private ISqlDataContext sqlContext;

        public TickStorage(
            IOptions<SqlConfiguration> sqlConfig,
            IKeyVaultManager kvManager,
            IDataSource dataSource)
        {
            this.dataSource = dataSource;
            this.sqlConfig = sqlConfig.Value;

            var connectionString = kvManager.GetSecretAsync(this.sqlConfig.ConnectionStringSecretName).Result;
            this.context = new TickContext(connectionString);
            this.sqlContext = new SqlDataContext(connectionString);
            this.context.Database.CreateIfNotExists();
            this.context.Configuration.AutoDetectChangesEnabled = false;
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

            var query = this.context.Ticks
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
                this.DeleteTicksForSymbol(symbol, interval);
                await this.SyncTickData(symbol, interval, start, end, tradeEndInclusive);
            }
            else
            {
                // if data was gathered from sql
                ticksStart = sqlTicks.FirstOrDefault().DateTime;
                ticksEnd = sqlTicks.LastOrDefault().DateTime;

                if (Helpers.Compare(ticksStart, start, interval) != 0
                || Helpers.Compare(ticksEnd, tradeEndInclusive, interval) != 0)
                {
                    Console.WriteLine($"Data from sql was insufficient date span. Fetching from source.");
                    this.DeleteTicksForSymbol(symbol, interval);
                    await this.SyncTickData(symbol, interval, start, end, tradeEndInclusive);
                }
            }

            query = this.context.Ticks
                .Where(t =>
                    t.Symbol == symbol
                    && t.TickInterval == interval
                    && t.DateTime >= start
                    && t.DateTime <= end)
                .OrderBy(t => t.DateTime);

            sqlTicks = query.ToList();

            DateTime sqlTicksStart = sqlTicks.FirstOrDefault().DateTime;
            DateTime sqlTicksEnd = sqlTicks.LastOrDefault().DateTime;

            if (Helpers.Compare(sqlTicksStart, start, interval) > 0
                || Helpers.Compare(sqlTicksEnd, tradeEndInclusive, interval) < 0)
            {
                Console.Error.WriteLine($"Data from source only spans {sqlTicksStart.StandardToString()} to {sqlTicksStart.StandardToString()}");
                //TODO: Fix the helper commands for date range check
                //throw new ArgumentOutOfRangeException();
            }

            return sqlTicks;
        }

        /// <summary>
        /// Sync SqlDataContext with data from the datasource
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="tradeEndInclusive"></param>
        /// <returns></returns>
        private async Task<IList<Tick>> SyncTickData(
            string symbol,
            TickInterval interval,
            DateTime start,
            DateTime end,
            DateTime tradeEndInclusive)
        {
            IList<Tick> sourceTicks = await dataSource.GetTicksAsync(symbol, interval, start, end);

            if (!sourceTicks.Any())
            {
                Console.Error.WriteLine($"Failed to fetch any data from source.");
                throw new Exception();
            }

            BulkInsertTicks(sourceTicks);          

            return sourceTicks;
        }

        /// <summary>
        /// Delete the rows in SqlDataContext that match the given symbol and tick interval
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        private void DeleteTicksForSymbol(string symbol, TickInterval interval)
        {
            string  sqlCommand = "DELETE FROM " + this.sqlConfig.TicksTableName + " WHERE [Symbol] = '" + symbol + "' AND [TickInterval] = " + (int)interval;
            int deletedRows = this.sqlContext.ExecuteCommand(sqlCommand);
            Console.WriteLine("Deleted " + deletedRows + " rows from dbo.[Ticks] where symbol = " + symbol);                
        }

        /// <summary>
        /// Insert all ticks to SqlDataContext
        /// </summary>
        /// <param name="ticks"></param>
        private void BulkInsertTicks(IEnumerable<Tick> ticks)
        {
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

            List<SqlBulkCopyColumnMapping> columnMappings = new List<SqlBulkCopyColumnMapping>()
            {
               new SqlBulkCopyColumnMapping("Symbol", "Symbol"),
               new SqlBulkCopyColumnMapping("DateTime", "DateTime"),
               new SqlBulkCopyColumnMapping("TickInterval", "TickInterval"),
               new SqlBulkCopyColumnMapping("Open", "Open"),
               new SqlBulkCopyColumnMapping("High", "High"),
               new SqlBulkCopyColumnMapping("Low", "Low"),
               new SqlBulkCopyColumnMapping("Close", "Close"),
               new SqlBulkCopyColumnMapping("AdjClose", "AdjClose"),
               new SqlBulkCopyColumnMapping("Volume", "Volume")

            };

            this.sqlContext.BulkInsert(data, columnMappings, this.sqlConfig.TicksTableName);
        }
    }
}
