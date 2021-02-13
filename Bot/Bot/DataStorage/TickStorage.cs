using Bot.Configuration;
using Bot.DataStorage.Models;
using Microsoft.Extensions.Options;
using System;

namespace Bot.DataStorage
{
    public class TickStorage : ITickStorage
    {
        SqlConfiguration sqlConfig;
        PriceContext context;

        public TickStorage(
            IOptions<SqlConfiguration> sqlConfig)
        {
            this.sqlConfig = sqlConfig.Value;
            context = new PriceContext(this.sqlConfig.ConnectionString);
            context.Database.CreateIfNotExists();
        }

        public void SaveTick(Tick tick)
        {
            Console.WriteLine("saving a tick to the db...");
            context.Ticks.Add(tick);
            context.SaveChanges();
        }
    }
}
