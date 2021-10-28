﻿
using System.Collections.Generic;

namespace Bot.Models
{
    public interface IMultiBar
    {
        /// <summary>
        /// Gets the latest ticker for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick this[string symbol] { get; }

        /// <summary>
        /// Gets the latest ticker for symbol at index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Tick this[int index] { get; }

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

        /// <summary>
        /// Returns the number of symbols.
        /// </summary>
        int NumSymbols { get; }
    }
}