using System;
using System.Collections.Generic;

namespace Bot.Models
{
    public class Portfolio
    {
        public Portfolio()
        {
            CashBalance = 0;
            Positions = new Dictionary<string, Position>();
        }

        public Portfolio(double initialCash)
        {
            CashBalance = initialCash;
            Positions = new Dictionary<string, Position>();
        }

        public Portfolio(IDictionary<string, Position> positions, double initialCash)
        {
            CashBalance = initialCash;
            Positions = positions;
        }

        /// <summary>
        /// Available cash balance.
        /// </summary>
        public double CashBalance { get; set; }

        /// <summary>
        /// symbol : position.
        /// </summary>
        public IDictionary<string, Position> Positions { get; set; }

        /// <summary>
        /// Gets the latest tick data for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position this[string symbol]
        {
            get => Positions[symbol];
        }

        /// <summary>
        /// Returns whether the symbol is in the positions list.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HasPosition(string symbol)
        {
            return Positions.ContainsKey(symbol);
        }

        /// <summary>
        /// Buys a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="quantity"></param>
        public void Buy(string symbol, double quantity, double price)
        {
            if (quantity == 0)
            {
                return;
            }

            if (Positions.ContainsKey(symbol))
            {
                Positions[symbol].Quantity += quantity;
                CashBalance -= price * quantity;

                if (Positions[symbol].Quantity == 0)
                {
                    Positions.Remove(symbol);
                }
            }
            else
            {
                Positions[symbol] = new Position(symbol, quantity);
                CashBalance -= price * quantity;
            }

        }

        /// <summary>
        /// Sells a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="quantity"></param>
        public void Sell(string symbol, double quantity, double price)
        {
            if (quantity == 0)
            {
                return;
            }

            if (Positions.ContainsKey(symbol))
            {
                Positions[symbol].Quantity -= quantity;
                CashBalance += price * quantity;

                if (Positions[symbol].Quantity == 0)
                {
                    Positions.Remove(symbol);
                }
            }
            else
            {
                Positions[symbol] = new Position(symbol, -quantity);
                CashBalance += price * quantity;
            }
        }

        /// <summary>
        /// Gets the current value of a portfolio based on current tick values;
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public double CurrentValue(ITicks ticks, Func<Tick, double> transform)
        {
            double value = 0;
            foreach(Position p in Positions.Values)
            {
                value += p.Quantity * transform(ticks[p.Symbol]);
            }
            return value + CashBalance;
        }
    }
}
