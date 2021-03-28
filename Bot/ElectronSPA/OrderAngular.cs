using Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronSPA
{
    public class OrderHistory
    {
        public string symbol { get; set; }

        public DateTime[] dates { get; set; }

        public double[] portfolioValue { get; set; }

        public double[] quantity { get; set; }

    }
}
