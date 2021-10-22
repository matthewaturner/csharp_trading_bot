using Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Data
{
    public interface IHistoricalDataSource
    {
        /// <summary>
        /// Gets a list of ticks for a certain interval over a period of time.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<IList<Tick>> GetHistoricalTicksAsync(
            string symbol,
            TickInterval interval,
            DateTime start,
            DateTime end);
    }
}
