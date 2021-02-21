
namespace Bot.Models
{
    public class Position
    {
        public Position()
        { }

        public Position(
            string symbol,
            double quantity)
        {
            Symbol = symbol.ToUpper();
            Quantity = quantity;
        }

        public string Symbol { get; set; }

        public double Quantity { get; set; }
    }
}
