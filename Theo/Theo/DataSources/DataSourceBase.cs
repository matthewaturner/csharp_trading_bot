using Theo.Data.Interfaces;
using Theo.Engine;
using Theo.Exceptions;
using Theo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Theo.Data
{
    public abstract class DataSourceBase : IDataSource, IHistoricalDataSource
    {
        /// <summary>
        /// Gets or sets engine object.
        /// </summary>
        /// <value></value>
        public ITradingEngine Engine { get; private set;}

        /// <summary>
        /// Each data source must override the initialize.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="args"></param>
        public void Initialize(ITradingEngine engine)
        {
            this.Engine = engine;
        }

        /// <summary>
        /// Stream ticks to the engine.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task StreamTicks(
            string[] symbols, 
            TickInterval interval, 
            DateTime start, 
            DateTime? end, 
            Action<Tick> callback)
        {
            IDictionary<string, IList<Tick>> allTicks = new Dictionary<string, IList<Tick>>(symbols.Length);
            end = end.HasValue ? end.Value : DateTime.UtcNow;

            foreach (string s in symbols)
            {
                allTicks[s] = await GetHistoricalTicksAsync(s, interval, start, end.Value);
            }

            int tickCount = allTicks.Values.First().Count();
            if (!allTicks.Values.All(ticks => ticks.Count() == tickCount))
            {
                throw new DataMisalignedException("Got a different number of ticks for each symbol.");
            }

            IList<IEnumerator<Tick>> enumerators = allTicks.Values.Select(ticks => ticks.GetEnumerator()).ToList();
            while (enumerators.All(e => e.MoveNext())
                && enumerators[0].Current.DateTime < end.Value)
            {
                foreach (var e in enumerators)
                {
                    try
                    {
                        callback(e.Current);
                    }
                    catch (Exception ex)
                    {
                        Engine.Logger.LogError(ex.ToString());
                    }
                }
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
