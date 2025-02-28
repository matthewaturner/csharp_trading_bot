using Bot.Models;
using Bot.Models.Interfaces;

namespace Bot.Brokers.BackTest.Models
{
    public class BackTestPosition : IPosition
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public BackTestPosition()
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="quantity"></param>
        public BackTestPosition(
            string symbol,
            decimal quantity)
        {
            Symbol = symbol.ToUpper();
            Quantity = quantity;
            Type = quantity > 0 ? PositionType.Long : PositionType.Short;
        }

        /// <summary>
        /// Symbol referred to.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Number of units. Can be fractional.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Long or short.
        /// </summary>
        public PositionType Type { get; set; }

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"(Symbol:{Symbol} Quantity:{Quantity})";
        }
    }
}
