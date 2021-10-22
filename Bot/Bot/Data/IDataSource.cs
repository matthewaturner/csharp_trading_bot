using Bot.Engine;
using Bot.Models;
using System;
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
        /// Stream ticks to the engine.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="onTickCallback"></param>
        /// <returns></returns>
        Task StreamTicks(
            string[] symbols,
            TickInterval interval,
            DateTime start,
            DateTime? end,
            Action<Tick[]> onTickCallback);
    }
}
