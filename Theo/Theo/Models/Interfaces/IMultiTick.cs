
using System.Collections.Generic;

namespace Theo.Models
{
    public interface IMultiTick
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
        /// ToString
        /// </summary>
        /// <returns></returns>
        string ToString();

        /// <summary>
        /// Returns the number of symbols.
        /// </summary>
        int NumSymbols { get; }
    }
}
