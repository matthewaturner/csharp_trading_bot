using System;
using System.Collections.Generic;
using System.Linq;

namespace Theo.Models
{
    public class MultiTick : IMultiTick
    {
        private IDictionary<string, Tick> ticks;
        private IList<string> symbols;

        /// <summary>
        /// Initializes ticks with no data.
        /// </summary>
        /// <param name="symbols"></param>
        public MultiTick(IList<string> symbols)
        {
            this.symbols = symbols;
            ticks = symbols.ToDictionary(keySelector: s => s, elementSelector: s => new Tick(s));
        }

        /// <summary>
        /// Initialize the ticks object.
        /// </summary>
        /// <param name="bars"></param>
        public MultiTick(Tick[] tickData)
        {
            this.ticks = new Dictionary<string, Tick>(tickData.Length);
            this.symbols = tickData.Select(t => t.Symbol).ToList();
            foreach (Tick t in tickData)
            {
                this.Update(t);
            }
        }

        /// <summary>
        /// Returns the number of symbols in the symbol map.
        /// </summary>
        public int NumSymbols => ticks.Keys.Count;

        /// <summary>
        /// Gets the latest tick data for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Tick this[string symbol]
        {
            get => ticks[symbol];
        }

        /// <summary>
        /// Updates current tick for just one symbol.
        /// </summary>
        /// <param name="newBars"></param>
        public void Update(Tick newTick)
        {
            // todo locking
            if (ticks.ContainsKey(newTick.Symbol))
            {
                ticks[newTick.Symbol] = newTick;
            }
            else
            {
                throw new Exception($"Got tick for symbol which was not in the original universe. " +
                    $"symbol={newTick.Symbol} universe={string.Join(",", this.symbols)}");
            }
        }

        /// <summary>
        /// Updates current ticks for multiple symbols.
        /// </summary>
        /// <param name="newTicks"></param>
        public void Update(IEnumerable<Tick> newTicks)
        {
            foreach (Tick t in newTicks)
            {
                this.Update(t);
            }
        }

        /// <summary>
        /// Returns whether or not we are collecting prices for this symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HasSymbol(string symbol)
        {
            return ticks.ContainsKey(symbol);
        }

        /// <summary>
        /// Print to the screen
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = "{";
            foreach (var kv in ticks)
            {
                output += kv.Value.ToString() + ", ";
            }
            return output + "}";
        }
    }
}
