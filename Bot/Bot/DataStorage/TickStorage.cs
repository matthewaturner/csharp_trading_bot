using Core;
using Bot.Configuration;
using Bot.DataStorage.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using Bot.DataCollection;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace Bot.DataStorage
{
    public class TickStorage : ITickStorage
    {
        SqlConfiguration sqlConfig;
        PriceContext context;
        IDataSource dataSource;

        public TickStorage(
            IOptions<SqlConfiguration> sqlConfig,
            IKeyVaultManager kvManager,
            IDataSource dataSource)
        {
            this.dataSource = dataSource;
            this.sqlConfig = sqlConfig.Value;

            string connectionString = kvManager.GetSecretAsync(this.sqlConfig.ConnectionStringSecretName).Result;
            context = new PriceContext(connectionString);
            context.Database.CreateIfNotExists();
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

            IList<Tick> ticks = query.ToList();
            DateTime ticksStart, ticksEnd;

            // if no data was gathered from sql
            if (ticks == null || !ticks.Any())
            {
                Console.WriteLine($"Data not found in database. Fetching from source.");
                ticks = await dataSource.GetTicksAsync(symbol, interval, start, end);

                if (!ticks.Any())
                {
                    Console.Error.WriteLine($"Failed to fetch any data from source.");
                    throw new Exception();
                }

                context.Ticks.AddRange(ticks);
                context.SaveChanges();

                ticksStart = ticks.FirstOrDefault().DateTime;
                ticksEnd = ticks.LastOrDefault().DateTime;

                if (Helpers.Compare(ticksStart, start, interval) != 0 
                    || Helpers.Compare(ticksEnd, tradeEndInclusive, interval) != 0)
                {
                    Console.Error.WriteLine($"Data from source only spans {ticksStart.StandardToString()} to {ticksEnd.StandardToString()}");
                    throw new ArgumentOutOfRangeException();
                }

                return ticks;
            }

            // if data was gathered from sql
            ticksStart = ticks.FirstOrDefault().DateTime;
            ticksEnd = ticks.LastOrDefault().DateTime;

            if (Helpers.Compare(ticksStart, start, interval) != 0 
                || Helpers.Compare(ticksEnd, tradeEndInclusive, interval) != 0)
            {
                Console.WriteLine($"Data from sql was insufficient date span. Fetching from source.");
                ticks = await dataSource.GetTicksAsync(symbol, interval, start, end);

                if (!ticks.Any())
                {
                    Console.Error.WriteLine($"Failed to fetch any data from source.");
                    throw new Exception();
                }

                // add or update everything
                foreach (Tick tick in ticks)
                {
                    context.Ticks.AddOrUpdate(tick);
                }
                context.SaveChanges();

                ticksStart = ticks.FirstOrDefault().DateTime;
                ticksEnd = ticks.LastOrDefault().DateTime;

                if (Helpers.Compare(ticksStart, start, interval) != 0 || Helpers.Compare(ticksEnd, end, interval) != 0)
                {
                    Console.Error.WriteLine($"Data from source only spans {ticksStart.StandardToString()} to {ticksEnd.StandardToString()}");
                    throw new ArgumentOutOfRangeException();
                }
            }

            return ticks;
        }
    }
}
