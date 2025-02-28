
namespace Bot.Models.Interfaces
{
    public interface IPosition
    {
        /// <summary>
        /// Symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Amount held.
        /// </summary>
        public decimal Quantity { get; }

        /// <summary>
        /// Type of position. Long or short.
        /// </summary>
        public PositionType Type { get; }
    }
}
