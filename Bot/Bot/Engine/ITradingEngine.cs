﻿using Bot.Brokers;
using Bot.Data.Interfaces;
using Bot.Models;
using Bot.Strategies;
using Bot.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Engine
{
    public interface ITradingEngine
    {
        /// <summary>
        /// Gets the symbols we are running for.
        /// </summary>
        public IList<string> Symbols { get; }

        /// <summary>
        /// Get current ticks.
        /// </summary>
        public IMultiTick Ticks { get; }

        /// <summary>
        /// Gets current data source.
        /// </summary>
        public IDataSource DataSource { get; }

        /// <summary>
        /// Get current broker.
        /// </summary>
        public IBroker Broker { get; }

        /// <summary>
        /// Gets current strategy.
        /// </summary>
        public IStrategy Strategy { get; }

        /// <summary>
        /// Get logger.
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Runs the strategy.
        /// </summary>
        /// <param name="runMode"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        Task RunAsync(
            RunMode runMode,
            TickInterval interval,
            DateTime? start = null,
            DateTime? end = null);
    }
}
