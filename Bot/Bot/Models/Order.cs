
using System;

namespace Bot.Models
{
    public class Order
    {
        public string OrderId { get; set; }

        public DateTime PlacementTime { get; set; }

        public DateTime ExecutionTime { get; set; }

        public string Symbol { get; set; }

        public double ExecutionPrice { get; set; }

        public double TargetPrice { get; set; }

        public double Quantity { get; set; }

        public OrderType Type { get; set; }

        public OrderStatus Status { get; set; }

        public void Fill(double price, DateTime executionTime)
        {
            ExecutionPrice = price;
            Status = OrderStatus.Filled;
            ExecutionTime = executionTime;
        }
    }
}
