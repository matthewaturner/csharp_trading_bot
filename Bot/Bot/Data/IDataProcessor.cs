using Bot.Models;
using System;
using Bot.Engine;

namespace Bot.Data
{
    public interface IDataProcessor
    {
        /// <summary>
        /// Initializes the data source with custom arguments.
        /// </summary>
        /// <param name="args"></param>
        void Initialize(ITradingEngine engine, string[] args);

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
