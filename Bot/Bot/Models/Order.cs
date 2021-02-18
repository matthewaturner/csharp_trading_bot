
using System;

namespace Bot.Models
{
    public class Order
    {
        public Order()
        { }

        public Order(
            string orderId,
            DateTime placementTime,
            DateTime executionTime,
            string symbol,
            double executionPrice,
            double targetPrice,
            double quantity,
            OrderType type,
            OrderState state)
        {
            OrderId = orderId;
            PlacementTime = placementTime;
            ExecutionTime = executionTime;
            Symbol = symbol.ToUpper();
            ExecutionPrice = executionPrice;
            TargetPrice = targetPrice;
            Quantity = quantity;
            Type = type;
            State = state;
        }

        public Order(OrderRequest request)
        {
            OrderId = Guid.NewGuid().ToString();
            PlacementTime = DateTime.Now;
            ExecutionTime = new DateTime();
            Symbol = request.Symbol.ToUpper();
            ExecutionPrice = double.NaN;
            TargetPrice = request.TargetPrice;
            Quantity = request.Quantity;
            Type = request.Type;
            State = OrderState.Open;
        }

        public string OrderId { get; set; }

        public DateTime PlacementTime { get; set; }

        public DateTime ExecutionTime { get; set; }

        public string Symbol { get; set; }

        public double ExecutionPrice { get; set; }

        public double TargetPrice { get; set; }

        public double Quantity { get; set; }

        public OrderType Type { get; set; }

        public OrderState State { get; set; }

        public void Fill(double price, DateTime executionTime)
        {
            ExecutionPrice = price;
            State = OrderState.Filled;
            ExecutionTime = executionTime;
        }
    }
}
