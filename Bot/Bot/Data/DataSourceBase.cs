using Bot.Engine;
using Bot.Exceptions;
using Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Data
{
    public abstract class DataSourceBase : IDataSource, IHistoricalDataSource, ILiveDataSource
    {
        /// <summary>
        /// Each data source must override the initialize.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public abstract void Initialize(ITradingEngine engine, string[] args);

        /// <summary>
        /// Stream ticks to the engine.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="onTickCallback"></param>
        /// <returns></returns>
        public async Task StreamTicks(
            string[] symbols, 
            TickInterval interval, 
            DateTime start, 
            DateTime? end, 
            Action<Tick[]> onTickCallback)
        {
            IList<Tick>[] allTicks = new List<Tick>[symbols.Length];
            int tickCount = 0;
            end = end.HasValue ? end.Value : DateTime.UtcNow;

            for (int i = 0; i < symbols.Length; i++)
            {
                allTicks[i] = await GetHistoricalTicksAsync(symbols[i], interval, start, end.Value);

                if (tickCount == 0)
                {
                    tickCount = allTicks[i].Count;
                }
                else if (allTicks[i].Count != tickCount)
                {
                    throw new BadDataException("Received varying number of ticks from each symbol.");
                }
            }

            Tick[] tickArray = new Tick[symbols.Length];
            MultiTick currentTicks = new MultiTick(symbols);
            IList<IEnumerator<Tick>> enumerators = new List<IEnumerator<Tick>>();

            for (int i = 0; i < symbols.Length; i++)
            {
                enumerators.Add(allTicks[i].GetEnumerator());
            }

            while (enumerators.All(e => e.MoveNext())
                && enumerators[0].Current.DateTime < end.Value)
            {
                for (int i = 0; i < enumerators.Count; i++)
                {
                    tickArray[i] = enumerators[i].Current;
                }

                onTickCallback(tickArray);
            }
        }

        /// <summary>
        /// Gets a list of ticks for a certain interval over a period of time.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public abstract Task<IList<Tick>> GetHistoricalTicksAsync(
            string symbol,
            TickInterval interval,
            DateTime start,
            DateTime end);
    }
}
