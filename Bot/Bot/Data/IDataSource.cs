using Bot.Models;
using Bot.Engine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Data
{
    public interface IDataSource
    {
        /// <summary>
        /// Initializes the data source with custom arguments.
        /// </summary>
        /// <param name="args"></param>
        void Initialize(ITradingEngine engine, string[] args);

        /// <summary>
        /// Gets a list of ticks for a certain interval over a period of time.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<IList<Tick>> GetTicksAsync(
            string symbol,
            TickInterval interval,
            DateTime start,
            DateTime end);
    }
}
