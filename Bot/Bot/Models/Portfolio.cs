using Bot.Models;
using System;
using System.Collections.Generic;

namespace Bot.Brokerage
{
    public class Portfolio
    {
        /// <summary>
        /// Instantiate a new portfolio with funds
        /// </summary>
        /// <param name="PortfolioFunds"></param>
        public Portfolio(double PortfolioFunds)
        {
            this.PortfolioId = Guid.NewGuid();
            this.Value = PortfolioFunds;
            this.AvailableCash = PortfolioFunds;
            this.TradeHistory = new List<Trade>();
            this.CurrentPositions = new Dictionary<string, Position>();
        }

        public Guid PortfolioId { get; set; }

        public double Value { get; set; }
        
        public double AvailableCash { get; set; }
        
        public List<Trade> TradeHistory { get; set; }
        
        public Dictionary<string, Position> CurrentPositions { get; set; }

        public void EnterPosition(Trade trade)
        {
            this.TradeHistory.Add(trade);
            var newPosition = new Position(trade.Ticker, PositionType.StockLong, trade.Units, trade.Price);
            this.CurrentPositions.Add(trade.Ticker, newPosition);
            this.AvailableCash -= trade.GetTradeValue();
        }

        public void ExitPosition(Trade trade)
        {
            this.TradeHistory.Add(trade);
            var pos = this.CurrentPositions[trade.Ticker];
            this.AvailableCash += trade.GetTradeValue();
            this.CurrentPositions.Remove(trade.Ticker);
        }
    }
}
