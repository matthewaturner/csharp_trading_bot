using System;

namespace Bot.Models
{
    public class Position
    {
        public Position(string symbol, int quantity, double entryPrice)
        {
            Symbol = symbol;
            Quantity = quantity;
            CostBasis = entryPrice;
        }

        public Position(Position p)
            : this(p.Symbol, p.Quantity, p.CostBasis)
        { }

        public string Symbol { get; set; }

        public int Quantity { get; set; }

        public double CostBasis { get; set; }

        public void Close(int quantity, double exitPrice)
        {
            Quantity -= quantity;
        }

        public void Open(int quantity, double entryPrice)
        {
            CostBasis = ((CostBasis * Quantity) + (entryPrice * quantity)) / (Quantity + quantity);
            Quantity += quantity;
        }

        public double Value(double currentPrice)
        {
            return Quantity*currentPrice;
        }
    }
}
