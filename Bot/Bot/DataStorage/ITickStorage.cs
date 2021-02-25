using Bot.DataCollection;
using Bot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bot.Engine;

namespace Bot.DataStorage
{
    public interface ITickStorage
    {
        /// <summary>
        /// Initializes the data source with custom arguments.
        /// </summary>
        /// <param name="args"></param>
        void Initialize(ITradingEngine engine, string[] args);

        /// <summary>
        /// Get ticks from a source for a symbol for some interval and over a range of time.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval">Tick interval.</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<IList<Tick>> GetTicksAsync(
            IDataSource source,
            string symbol,
            TickInterval interval,
            DateTime start,
            DateTime end);

        /// <summary>
        /// Streams ticks 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="symbols"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        void StreamTicks(
            IDataSource source,
            string[] symbols,
            TickInterval interval,
            DateTime start,
            DateTime end,
            Action<Tick[]> onTickCallback);
    }
}
