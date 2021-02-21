
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

        bool HasSymbol(string symbol);
    }
}
