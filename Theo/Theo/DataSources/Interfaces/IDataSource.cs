﻿using Theo.Engine.Events;
using Theo.Engine;
using Theo.Models;
using System.Threading.Tasks;
using System;

namespace Theo.Data.Interfaces
{
    public interface IDataSource : IInitialize
    {
        /// <summary>
        /// Stream bars to the engine.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="callback"></param>
        /// <returns>Calls some callback for all bars.</returns>
        Task StreamBars(
            string[] symbols,
            DataInterval interval,
            DateTime start,
            DateTime? end,
            Action<Bar> callback);
    }
}
