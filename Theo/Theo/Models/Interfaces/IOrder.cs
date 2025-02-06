using System;

namespace Theo.Models.Interfaces
{
    public interface IOrder
    {
        public string OrderId { get; }

        public DateTime PlacementTime { get; }

        public DateTime ExecutionTime { get; }

        public string Symbol { get; }

        public double Quantity { get; }

        public double TargetPrice { get; }

        public double AverageFillPrice { get; }

        public OrderType Type { get; }

        public OrderState State { get; }
    }
}
