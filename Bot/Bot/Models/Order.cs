
using System;

namespace Bot.Models
{
    public class Order
    {
        public string OrderId { get; set; }

        public DateTime Date { get; set; }

        public string Symbol { get; set; }

        public double ExecutionPrice { get; set; }

        public double LastPrice { get; set; }

        public double Units { get; set; }

        public OrderType Type { get; set; }
    }
}
