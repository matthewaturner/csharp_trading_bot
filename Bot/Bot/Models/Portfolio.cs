using Bot.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models
{
    public class Portfolio
    {
        public Portfolio()
        {
            Positions = new Dictionary<string, Position>();
            CashBalance = 0;
        }

        public Portfolio(double initialCash)
        {
            Positions = new Dictionary<string, Position>();
            CashBalance = initialCash;
        }

        public Portfolio(IDictionary<string, Position> positions, double initialCash)
        {
            Positions = positions;
            CashBalance = initialCash;
        }

        public double CashBalance { get; set; }

        public IDictionary<string, Position> Positions { get; set; }

        public double GetTotalValue(IDictionary<string, double> currentPrices)
        {
            double totalValue = 0;

            foreach(KeyValuePair<string, Position> kvp in Positions)
            {
                if (!currentPrices.ContainsKey(kvp.Key))
                {
                    throw new ArgumentNullException("currentPrices must contain prices for all held symbols.");
                }

                totalValue += kvp.Value.Quantity * currentPrices[kvp.Key];
            }

            return totalValue;
        }

        public void BuySymbol(string symbol, int quantity, double currentPrice)
        {
            if (quantity < 1)
            {
                throw new InvalidOrderException("Quantity cannot be less than 1.");
            }

            if (CashBalance < quantity*currentPrice)
            {
                throw new InvalidOrderException("Order exceeds cash value of portfolio.");
            }

            // buy
            if (Positions.ContainsKey(symbol))
            {
                Positions[symbol].Buy(quantity);
            }
            else
            {
                Positions[symbol] = new Position(symbol, quantity);
            }

            // remove zero positions
            if (Positions[symbol].Quantity == 0)
            {
                Positions.Remove(symbol);
            }

            CashBalance -= quantity*currentPrice;
        }

        public void SellSymbol(string symbol, int quantity, double currentPrice)
        {
            if (quantity < 1)
            {
                throw new InvalidOrderException("Quantity cannot be less than 1.");
            }

            if (Positions.ContainsKey(symbol))
            {
                Positions[symbol].Sell(quantity);
            }
            else
            {
                Positions[symbol] = new Position(symbol, -quantity);
            }

            // remove zero positions
            if (Positions[symbol].Quantity == 0)
            {
                Positions.Remove(symbol);
            }


            CashBalance += quantity*currentPrice;
        }
    }
}
