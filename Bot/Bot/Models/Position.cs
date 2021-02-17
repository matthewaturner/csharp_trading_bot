
namespace Bot.Models
{
    public class Position
    {
        public Position(string symbol, double quantity)
        {
            Symbol = symbol;
            Quantity = quantity;
        }

        public Position(Position positionToCopy)
        {
            Symbol = positionToCopy.Symbol;
            Quantity = positionToCopy.Quantity;
        }

        public string Symbol { get; set; }

        public double Quantity { get; set; }

        public void Sell(double quantity)
        {
            Quantity -= quantity;
        }

        public void Buy(double quantity)
        {
            Quantity += quantity;
        }

        public double Value(double currentPrice)
        {
            return Quantity*currentPrice;
        }
    }
}
