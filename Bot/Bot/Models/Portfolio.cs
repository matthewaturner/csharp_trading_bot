using Bot.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models
{
    public class Portfolio
    {
        public Portfolio()
        {
            Positions = new List<Position>();
            CashBalance = 0;
        }

        public Portfolio(double initialFunds)
        {
            Positions = new List<Position>();
            CashBalance = initialFunds;
        }

        public Portfolio(IList<Position> positions, double initialFunds)
        {
            Positions = positions;
            CashBalance = initialFunds;
        }

        public double TotalValue { get; set; }

        public double CashBalance { get; set; }

        public IList<Position> Positions { get; set; }

        public void OpenPosition(string symbol, int quantity, double entryPrice)
        {
            if (quantity < 1)
            {
                throw new InvalidOrderException("Quantity cannot be less than 1.");
            }

            if (HoldingSymbol(symbol))
            {
                Positions[GetPositionIndex(symbol)].Open(quantity, entryPrice);
            }
            else
            {
                Positions.Add(new Position(symbol, quantity, entryPrice));
            }

            CashBalance -= quantity * entryPrice;
        }

        public void ClosePosition(string symbol, int quantity, double exitPrice)
        {
            if (!HoldingSymbol(symbol))
            {
                throw new InvalidOrderException("Cannot close a position which is not held.");
            }

            if (quantity < 1)
            {
                throw new InvalidOrderException("Quantity cannot be less than 1.");
            }

            int index = GetPositionIndex(symbol);
            Positions[index].Close(quantity, exitPrice);
            if (Positions[index].Quantity == 0)
            {
                Positions.RemoveAt(index);
            }

            CashBalance += quantity * exitPrice;
        }

        public bool HoldingSymbol(string symbol)
        {
            return Positions.Any(pos => string.CompareOrdinal(pos.Symbol, symbol) == 0);
        }

        public int GetPositionIndex(string symbol)
        {
            return Positions.TakeWhile(pos => string.CompareOrdinal(pos.Symbol, symbol) != 0).Count();
        }
    }
}
