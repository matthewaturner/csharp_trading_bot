using Bot.Exceptions;
using System;
using System.Collections.Generic;

namespace Bot.Models
{
    public class Portfolio
    {
        private ICurrentTicks currentTicks;

        /// <summary>
        /// Null constructor.
        /// </summary>
        public Portfolio(ICurrentTicks currentTicks)
        {
            this.currentTicks = currentTicks;
            Positions = new Dictionary<string, Position>();
            CashBalance = 0;
        }

        /// <summary>
        /// Constructs a portfolio with some initial cash.
        /// </summary>
        /// <param name="initialCash"></param>
        public Portfolio(ICurrentTicks currentTicks, double initialCash)
        {
            this.currentTicks = currentTicks;
            Positions = new Dictionary<string, Position>();
            CashBalance = initialCash;
        }

        /// <summary>
        /// Constructs a portfolio.
        /// </summary>
        /// <param name="positions"></param>
        /// <param name="initialCash"></param>
        public Portfolio(ICurrentTicks currentTicks, IDictionary<string, Position> positions, double initialCash)
        {
            this.currentTicks = currentTicks;
            Positions = positions;
            CashBalance = initialCash;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="portfolioToCopy"></param>
        public Portfolio(Portfolio portfolioToCopy)
        {
            // copy the value types
            currentTicks = portfolioToCopy.currentTicks;
            CashBalance = portfolioToCopy.CashBalance;

            // make a deep copy of positions
            Positions = new Dictionary<string, Position>();
            foreach (Position p in portfolioToCopy.Positions.Values)
            {
                Positions.Add(p.Symbol, new Position(p));
            }
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
        /// Gets the total value of the portfolio based on current prices.
        /// </summary>
        /// <param name="currentPrices"></param>
        /// <returns></returns>
        public double GetTotalValue()
        {
            double totalValue = 0;

            foreach(KeyValuePair<string, Position> kvp in Positions)
            {
                totalValue += kvp.Value.Quantity * currentTicks[kvp.Key].Close;
            }

            return totalValue;
        }

        /// <summary>
        /// Buys a symbol at a given price.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="quantity"></param>
        /// <param name="currentPrice"></param>
        public void ExecuteOrder(Order order, double executionPrice)
        {
            if (0 < order.Quantity && order.Quantity < 1)
            {
                throw new InvalidOrderException("Quantity cannot be less than 1.");
            }

            if (CashBalance < order.Quantity*order.ExecutionPrice)
            {
                throw new InvalidOrderException("Order exceeds cash value of portfolio.");
            }

            // buy or sell existing position
            if (Positions.ContainsKey(order.Symbol))
            {
                if (order.Type == OrderType.Buy)
                {
                    CashBalance -= executionPrice * order.Quantity;
                    Positions[order.Symbol].Buy(order.Quantity);
                }
                else if (order.Type == OrderType.Sell)
                {
                    CashBalance += executionPrice * order.Quantity;
                    Positions[order.Symbol].Sell(order.Quantity);
                }
                else
                {
                    throw new NotImplementedException($"OrderType {order.Type} not implemented.");
                }

                // remove zero positions
                if (Positions[order.Symbol].Quantity == 0)
                {
                    Positions.Remove(order.Symbol);
                }
            }
            else
            {
                // new position
                Positions[order.Symbol] = new Position(order.Symbol, order.Quantity);
            }

            CashBalance -= order.Quantity*order.ExecutionPrice;
        }

        /// <summary>
        /// Copies the portfolio and executes an order on it.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="executionPrice"></param>
        /// <returns></returns>
        public Portfolio SimulateOrder(Order order, double executionPrice)
        {
            Portfolio copy = new Portfolio(this);
            copy.ExecuteOrder(order, executionPrice);
            return copy;
        }

        /// <summary>
        /// Adds funds to a portfolio.
        /// </summary>
        /// <param name="cash"></param>
        public void AddFunds(double cash)
        {
            CashBalance += cash;
        }

        /// <summary>
        /// Validates that it is possible to execute an order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool ValidateOrder(Order order, double executionPrice, out string message)
        {
            if (.000001 < executionPrice && executionPrice < .000001)
            {
                message = "executionPrice cannot be zero.";
                return false;
            }

            message = string.Empty;
            double orderValue = executionPrice * order.Quantity;

            switch (order.Type)
            {
                case OrderType.Buy:
                    if (CashBalance < orderValue)
                    {
                        message = "Not enough cash to cover buy.";
                        return false;
                    }
                    return true;

                case OrderType.Sell:
                    /*
                    double netQuantity = Positions[order.Symbol].Quantity - order.Quantity;
                    double netPositionValue = netQuantity * order.ExecutionPrice;
                    double cashAfterTrade = CashBalance += orderValue;
                    if (netQuantity < 0)
                    {
                        if (-netPositionValue > (GetTotalValue() + cashAfterTrade)/2)
                        {
                            message = "Cannot be short more than 50% of total account value.";
                            return false;
                        }
                    }
                    */
                    return true;
            }

            return true;
        }
    }
}
