﻿using Bot.DataStorage.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.DataCollection
{
    public interface IDataSource
    {
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