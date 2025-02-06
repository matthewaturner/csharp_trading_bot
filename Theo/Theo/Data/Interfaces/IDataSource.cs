using Theo.Engine.Events;
using Theo.Engine;
using Theo.Models;
using System.Threading.Tasks;
using System;

namespace Theo.Data.Interfaces
{
    public interface IDataSource : IInitialize
    {
        /// <summary>
        /// Stream ticks to the engine.
        /// </summary>
        /// <param name="symbols"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="callback"></param>
        /// <returns>Calls some callback for all ticks.</returns>
        Task StreamTicks(
            string[] symbols,
            TickInterval interval,
            DateTime start,
            DateTime? end,
            Action<Tick> callback);
    }
}
