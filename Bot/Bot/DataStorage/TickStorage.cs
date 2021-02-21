using Core;
using Bot.Configuration;
using Bot.DataStorage.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Bot.DataCollection;
using System.Threading.Tasks;
using System.Data;
using Bot.Engine;

namespace Bot.DataStorage
{
    public class TickStorage : ITickStorage
    {
        private SqlConfiguration sqlConfig;
        private TickContext context;

        public TickStorage(
            IOptions<SqlConfiguration> sqlConfig,
            IKeyVaultManager kvManager)
        {
            this.sqlConfig = sqlConfig.Value;

            var connectionString = kvManager.GetSecretAsync(this.sqlConfig.ConnectionStringSecretName).Result;
            this.context = new TickContext(connectionString, this.sqlConfig.TicksTableName);
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
            IDataSource source,
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
                context.DeleteTicksForSymbol(symbol, interval);
                await SyncTickData(source, symbol, interval, start, end);
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
                    context.DeleteTicksForSymbol(symbol, interval);
                    await SyncTickData(source, symbol, interval, start, end);
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
        /// <returns></returns>
        private async Task<IList<Tick>> SyncTickData(
            IDataSource source,
            string symbol,
            TickInterval interval,
            DateTime start,
            DateTime end)
        {
            IList<Tick> sourceTicks = await source.GetTicksAsync(symbol, interval, start, end);

            if (!sourceTicks.Any())
            {
                Console.Error.WriteLine($"Failed to fetch any data from source.");
                throw new Exception();
            }

            context.BulkInsert(sourceTicks);

            return sourceTicks;
        }
    }
}
