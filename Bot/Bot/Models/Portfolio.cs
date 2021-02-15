using System.Collections.Generic;

namespace Bot.Models
{
    public class Portfolio
    {
        public double TotalValue { get; set; }

        public double CashBalance { get; set; }

        public IList<Position> Positions { get; set; }
    }
}
