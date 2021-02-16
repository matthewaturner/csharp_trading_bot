
namespace Bot.Models
{
    public class Position
    {
        public Position(string symbol, int quantity)
        {
            Symbol = symbol;
            Quantity = quantity;
        }

        public string Symbol { get; set; }

        public int Quantity { get; set; }

        public void Sell(int quantity)
        {
            Quantity -= quantity;
        }

        public void Buy(int quantity)
        {
            Quantity += quantity;
        }

        public double Value(double currentPrice)
        {
            return Quantity*currentPrice;
        }
    }
}
