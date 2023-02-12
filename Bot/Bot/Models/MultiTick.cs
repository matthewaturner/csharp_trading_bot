using System;
using System.Collections.Generic;

namespace Bot.Models
{
    public class MultiTick : IMultiTick
    {
        private Tick[] bars;
        private IDictionary<string, int> symbolMap;

        /// <summary>
        /// Initializes ticks with no data.
        /// </summary>
        /// <param name="symbols"></param>
        public MultiTick(string[] symbols)
        {
            bars = new Tick[symbols.Length];

            symbolMap = new Dictionary<string, int>(symbols.Length);
            for (int i=0; i<symbols.Length; i++)
            {
                bars[i] = new Tick();
                bars[i].Symbol = symbols[i];
                symbolMap[symbols[i]] = i;
            }
        }

        /// <summary>
        /// Initialize the ticks object.
        /// </summary>
        /// <param name="bars"></param>
        public MultiTick(Tick[] bars)
        {
            this.bars = bars;

            for (int i=0; i<bars.Length; i++)
            {
                symbolMap[bars[i].Symbol] = i;
            }
        }

        /// <summary>
        /// Returns the number of symbols in the symbol map.
        /// </summary>
        public int NumSymbols => bars.Length;

        /// <summary>
        /// Gets the latest tick data for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Tick this[string symbol]
        {
            get => bars[symbolMap[symbol]];
        }

        /// <summary>
        /// Gets the latest tick for symbol at index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Tick this[int index]
        {
            get => bars[index];
        }

        /// <summary>
        /// Updates the current prices. Called by engine.
        /// </summary>
        /// <param name="newBars"></param>
        public void Update(Tick[] newBars)
        {
            // iterate this way because we should never add ticks
            // in the middle of a run

            if (newBars.Length != bars.Length)
            {
                throw new ArgumentOutOfRangeException("new bars contain more or less bars than previously.");
            }

            for (int i=0; i<bars.Length; i++)
            {
                if (string.Compare(bars[i].Symbol, newBars[i].Symbol, ignoreCase: true) != 0)
                {
                    throw new ArgumentException($"newBars[{i}] was for symbol {newBars[i].Symbol}. Expected {bars[i].Symbol}");
                }
                bars[i] = newBars[i];
            }
        }

        /// <summary>
        /// Returns whether or not we are collecting prices for this symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HasSymbol(string symbol)
        {
            return symbolMap.ContainsKey(symbol);
        }

        /// <summary>
        /// Returns ticks as a list.
        /// </summary>
        /// <returns></returns>
        public Tick[] ToArray()
        {
            return bars;
        }

        /// <summary>
        /// Print to the screen
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = string.Empty;
            for (int i=0; i<bars.Length; i++)
            {
                output += bars[i].ToString() + "\n";
            }
            return output;
        }
    }
}
