using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Engine
{
    public class Ticks : ITicks
    {
        private Dictionary<string, Tick> currentTicks;

        public Ticks()
        { }

        /// <summary>
        /// Initializes the ticks object to have all symbols.
        /// </summary>
        /// <param name="symbols"></param>
        public void Initialize(string[] symbols)
        {
            currentTicks = new Dictionary<string, Tick>();
            foreach (string s in symbols)
            {
                currentTicks.Add(s.ToUpper(), null);
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
            foreach (string symbol in currentTicks.Keys.ToList())
            {
                string upperSymbol = symbol.ToUpper();
                if (!newTicks.ContainsKey(symbol.ToUpper()))
                {
                    throw new ArgumentException("Ticks must contain data for all ticks in the universe.");
                }

                currentTicks[symbol.ToUpper()] = newTicks[symbol.ToUpper()];
            }
        }

        /// <summary>
        /// Returns whether or not we are collecting prices for this symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HasSymbol(string symbol)
        {
            return currentTicks.ContainsKey(symbol.ToUpper());
        }

        /// <summary>
        /// Returns ticks as a list.
        /// </summary>
        /// <returns></returns>
        public IList<Tick> ToList()
        {
            return currentTicks.Values.ToList();
        }
    }
}
