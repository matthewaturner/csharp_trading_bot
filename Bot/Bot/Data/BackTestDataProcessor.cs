using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Bot.Models;
using Bot.Engine;

namespace Bot.Data
{
    public class BackTestDataProcessor : IDataProcessor
    {
        private IDataSource dataSource;

        /// <summary>
        /// Dependency injection.
        /// </summary>
        /// <param name="sqlConfig"></param>
        /// <param name="kvManager"></param>
        public BackTestDataProcessor()
        { }

        /// <summary>
        /// Initializes the data source with custom arguments.
        /// </summary>
        /// <param name="args"></param>
        public void Initialize(ITradingEngine engine, string[] args)
        {
            dataSource = engine.DataSource;
        }

        /// <summary>
        /// Call get ticks for each symbol and unify into one multi-tick stream of events.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="symbols"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="onTickCallback"></param>
        public async void StreamTicks(
            IDataSource source,
            string[] symbols,
            TickInterval interval,
            DateTime start,
            DateTime end,
            Action<Tick[]> onTickCallback)
        {
            IList<Tick>[] allTicks = new List<Tick>[symbols.Length];
            int tickCount = 0;

            for (int i = 0; i < symbols.Length; i++)
            {
                allTicks[i] = await dataSource.GetTicksAsync(symbols[i], interval, start, end);

                if (tickCount == 0)
                {
                    tickCount = allTicks[i].Count;
                }
                else if (allTicks[i].Count != tickCount)
                {
                    throw new DataException("Received varying number of ticks from each symbol.");
                }
            }

            Tick[] tickArray = new Tick[symbols.Length];
            Ticks currentTicks = new Ticks(symbols);
            IList<IEnumerator<Tick>> enumerators = new List<IEnumerator<Tick>>();

            for (int i = 0; i < symbols.Length; i++)
            {
                enumerators.Add(allTicks[i].GetEnumerator());
            }

            while (enumerators.All(e => e.MoveNext()))
            {
                for (int i = 0; i < enumerators.Count; i++)
                {
                    tickArray[i] = enumerators[i].Current;
                }

                onTickCallback(tickArray);
            }
        }
    }
}
