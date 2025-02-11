using Theo.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Theo.Data.Interfaces
{
    public interface IHistoricalDataSource
    {
        /// <summary>
        /// Gets a list of bars for a certain interval over a period of time.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task<IList<Bar>> GetHistoricalBarsAsync(
            string symbol,
            DataInterval interval,
            DateTime start,
            DateTime end);
    }
}
