using Bot.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.DataStorage
{
    public interface ITickStorage
    {
        /// <summary>
        /// Get ticks for a symbol for some interval and over a range of time.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval">Tick interval.</param>
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
