using System.Data.Entity;

namespace Bot.DataStorage.Models
{
    public class TickContext : DbContext
    {
        public TickContext(string connectionString)
            : base(connectionString)
        { }

        public DbSet<Tick> Ticks { get; set; }
    }
}
