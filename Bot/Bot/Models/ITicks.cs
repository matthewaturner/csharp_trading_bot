
using System.Collections.Generic;

namespace Bot.Models
{
    public interface ITicks
    {
        /// <summary>
        /// Gets the latest ticker for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick this[string symbol] { get; }

        /// <summary>
        /// Returns whether we have a tick for this symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        bool HasSymbol(string symbol);

        /// <summary>
        /// Returns all ticks as an array.
        /// </summary>
        /// <returns></returns>
        Tick[] ToArray();

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}
