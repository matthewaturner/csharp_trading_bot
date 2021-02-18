using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Models
{
    public class CurrentTicks : ICurrentTicks
    {
        private Dictionary<string, Tick> currentTicks;

        public CurrentTicks(string[] symbols)
        {
            currentTicks = new Dictionary<string, Tick>();
            foreach (string s in symbols)
            {
                currentTicks.Add(s, null);
            }
        }

        /// <summary>
        /// Gets the latest tick data for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Tick this[string symbol]
        {
            get => currentTicks[symbol];
        }

        /// <summary>
        /// Updates the current prices. Called by engine.
        /// </summary>
        /// <param name="newTicks"></param>
        public void Update(Dictionary<string, Tick> newTicks)
        {
            // iterate this way because we should never add ticks
            // in the middle of a run
            foreach (KeyValuePair<string, Tick> kvp in currentTicks)
            {
                if (!newTicks.ContainsKey(kvp.Key))
                {
                    throw new ArgumentException("Ticks must contain data for all ticks in the universe.");
                }

                currentTicks[kvp.Key] = newTicks[kvp.Key];
            }
        }

        /// <summary>
        /// Returns whether or not we are collecting prices for this symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HasSymbol(string symbol)
        {
            return currentTicks.ContainsKey(symbol);
        }
    }
}
