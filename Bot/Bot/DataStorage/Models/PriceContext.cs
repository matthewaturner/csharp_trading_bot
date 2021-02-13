using System.Data.Entity;

namespace Bot.DataStorage.Models
{
    public class PriceContext : DbContext
    {
        public PriceContext(string connectionString)
            : base(connectionString)
        { }

        public DbSet<Tick> Ticks { get; set; }
    }
}
