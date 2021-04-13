using Bot.Brokers.BackTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models
{
    public class Portfolio
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Portfolio()
        {
            CashBalance = 0;
            Positions = new Dictionary<string, BackTestPosition>();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="initialCash"></param>
        public Portfolio(double initialCash)
        {
            CashBalance = initialCash;
            Positions = new Dictionary<string, BackTestPosition>();
        }

        /// <summary>
        /// Available cash balance.
        /// </summary>
        public double CashBalance { get; private set; }

        /// <summary>
        /// symbol : position.
        /// </summary>
        public IDictionary<string, BackTestPosition> Positions { get; set; }

        /// <summary>
        /// Gets the latest tick data for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public BackTestPosition this[string symbol]
        {
            get => Positions[symbol];
        }

        /// <summary>
        /// Adds cash to portfolio.
        /// </summary>
        /// <param name="cash"></param>
        /// <returns></returns>
        public void AddFunds(double cash)
        {
            CashBalance += cash;
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
                Positions[symbol] = new BackTestPosition(symbol, quantity);
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
                Positions[symbol] = new BackTestPosition(symbol, -quantity);
                CashBalance += price * quantity;
            }
        }

        /// <summary>
        /// Gets the current value of a portfolio based on current tick values;
        /// </summary>
        /// <param name="ticks"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public double CurrentValue(IMultiTick ticks, Func<Tick, double> transform)
        {
            double value = 0;
            foreach(BackTestPosition p in Positions.Values)
            {
                value += p.Quantity * transform(ticks[p.Symbol]);
            }
            return value + CashBalance;
        }

        /// <summary>
        /// ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string positionsStr = string.Join(", ", Positions.Values.Select(pos => pos.ToString()));

            return $"{{CashBalance: {CashBalance}\nPositions: [{positionsStr}]}}";
        }
    }
}
