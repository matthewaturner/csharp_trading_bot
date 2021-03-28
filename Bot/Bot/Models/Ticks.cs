using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models
{
    public class Ticks : ITicks
    {
        private Tick[] ticks;
        private IDictionary<string, int> symbolMap;

        /// <summary>
        /// Initializes ticks with no data.
        /// </summary>
        /// <param name="symbols"></param>
        public Ticks(string[] symbols)
        {
            ticks = new Tick[symbols.Length];

            symbolMap = new Dictionary<string, int>(symbols.Length);
            for (int i=0; i<symbols.Length; i++)
            {
                ticks[i] = new Tick();
                ticks[i].Symbol = symbols[i];
                symbolMap[symbols[i]] = i;
            }
        }

        /// <summary>
        /// Initialize the ticks object.
        /// </summary>
        /// <param name="ticks"></param>
        public Ticks(Tick[] ticks)
        {
            this.ticks = ticks;

            for (int i=0; i<ticks.Length; i++)
            {
                symbolMap[ticks[i].Symbol] = i;
            }
        }

        /// <summary>
        /// Gets the latest tick data for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Tick this[string symbol]
        {
            get => ticks[symbolMap[symbol]];
        }

        /// <summary>
        /// Gets the latest tick for symbol at index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Tick this[int index]
        {
            get => ticks[index];
        }

        /// <summary>
        /// Updates the current prices. Called by engine.
        /// </summary>
        /// <param name="newTicks"></param>
        public void Update(Tick[] newTicks)
        {
            // iterate this way because we should never add ticks
            // in the middle of a run

            if (newTicks.Length != ticks.Length)
            {
                throw new ArgumentOutOfRangeException("new ticks contain more or less ticks than previously.");
            }

            for (int i=0; i<ticks.Length; i++)
            {
                if (string.Compare(ticks[i].Symbol, newTicks[i].Symbol, ignoreCase: true) != 0)
                {
                    throw new ArgumentException($"newTicks[{i}] was for symbol {newTicks[i].Symbol}. Expected {ticks[i].Symbol}");
                }
                ticks[i] = newTicks[i];
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
            return ticks;
        }

        /// <summary>
        /// Print to the screen
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = string.Empty;
            for (int i=0; i<ticks.Length; i++)
            {
                output += ticks[i].ToString() + "\n";
            }
            return output;
        }
    }
}
