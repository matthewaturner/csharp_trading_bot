
namespace Bot.Engine
{
    public class Position
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Position()
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="quantity"></param>
        public Position(
            string symbol,
            double quantity)
        {
            Symbol = symbol.ToUpper();
            Quantity = quantity;
        }

        /// <summary>
        /// Symbol referred to.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Number of units. Can be fractional.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"(Symbol:{Symbol} Quantity:{Quantity})";
        }

        public PositionType GetPositionType()
        {
            return this.Quantity > 0 ? PositionType.Long : PositionType.Short;
        }
    }
}
