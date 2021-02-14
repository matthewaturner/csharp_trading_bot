using Bot.Trading.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Trading
{
    public class Trade : ITrade
    {
        public Guid TradeId { get; set; }

        public DateTime Date { get; set; }

        public string Ticker { get; set; }

        public double Price { get; set; }

        public double Units { get; set; }

        public TradeType TradeType { get; set; }

        public double GetTradeValue()
        {
            return Math.Abs(this.Units * this.Price);
        }
    }
}
