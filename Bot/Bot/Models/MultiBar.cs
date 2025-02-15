using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models
{
    public class MultiBar
    {
        private IDictionary<string, Bar> bars;
        private IList<string> symbols;

        /// <summary>
        /// Initializes bars with no data.
        /// </summary>
        /// <param name="symbols"></param>
        public MultiBar(IList<string> symbols)
        {
            this.symbols = symbols;
            bars = symbols.ToDictionary(
                keySelector: s => s, 
                elementSelector: s => new Bar(DateTime.MinValue, s, 0, 0, 0, 0, 0));
        }

        /// <summary>
        /// Initialize the bars object.
        /// </summary>
        /// <param name="bars"></param>
        public MultiBar(Bar[] barData)
        {
            this.bars = new Dictionary<string, Bar>(barData.Length);
            this.symbols = barData.Select(t => t.Symbol).ToList();
            foreach (Bar t in barData)
            {
                this.Update(t);
            }
        }

        /// <summary>
        /// Returns the number of symbols in the symbol map.
        /// </summary>
        public int NumSymbols => bars.Keys.Count;

        /// <summary>
        /// Gets the latest bar data for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Bar this[string symbol]
        {
            get => bars[symbol];
        }

        /// <summary>
        /// Updates current bar for just one symbol.
        /// </summary>
        /// <param name="newBars"></param>
        public void Update(Bar newBar)
        {
            // todo locking
            if (bars.ContainsKey(newBar.Symbol))
            {
                bars[newBar.Symbol] = newBar;
            }
            else
            {
                throw new Exception($"Got bar for symbol which was not in the original universe. " +
                    $"symbol={newBar.Symbol} universe={string.Join(",", this.symbols)}");
            }
        }

        /// <summary>
        /// Updates current bars for multiple symbols.
        /// </summary>
        /// <param name="newBars"></param>
        public void Update(IEnumerable<Bar> newBars)
        {
            foreach (Bar t in newBars)
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
            return bars.ContainsKey(symbol);
        }

        /// <summary>
        /// Print to the screen
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string output = "{";
            foreach (var kv in bars)
            {
                output += kv.Value.ToString() + ", ";
            }
            return output + "}";
        }
    }
}
