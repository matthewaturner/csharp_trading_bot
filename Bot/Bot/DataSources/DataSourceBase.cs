using Bot.Engine;
using Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DataSources
{
    public abstract class DataSourceBase : IDataSource
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
        /// Stream bars to the engine.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public async Task StreamBars(
            string[] symbols, 
            Interval interval, 
            DateTime start, 
            DateTime? end, 
            Action<Bar> callback)
        {
            IDictionary<string, IList<Bar>> allBars = new Dictionary<string, IList<Bar>>(symbols.Length);
            end = end.HasValue ? end.Value : DateTime.UtcNow;

            foreach (string s in symbols)
            {
                allBars[s] = await GetHistoricalBarsAsync(s, interval, start, end.Value);
            }

            int barCount = allBars.Values.First().Count();
            if (!allBars.Values.All(bars => bars.Count() == barCount))
            {
                throw new DataMisalignedException("Got a different number of bars for each symbol.");
            }

            IList<IEnumerator<Bar>> enumerators = allBars.Values.Select(bars => bars.GetEnumerator()).ToList();
            while (enumerators.All(e => e.MoveNext()) && enumerators[0].Current.Timestamp < end.Value)
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
        /// Gets a list of bars for a symbol, interval, and period of time.
        /// </summary>
        public abstract Task<List<Bar>> GetHistoricalBarsAsync(
            string symbol, 
            Interval interval, 
            DateTime start, 
            DateTime end);
    }
}
