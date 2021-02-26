using Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectronSPA
{
    public class OrderAngular
    {
        public string orderId { get; set; }
        
        public DateTime placementTime { get; set; }
        
        public DateTime executionTime { get; set; }
        
        public string symbol { get; set; }
        
        public double executionPrice { get; set; }

        public double targetPrice { get; set; }

        public double quantity { get; set; }

        public int type { get; set; }

        public int state { get; set; }

    }
}
