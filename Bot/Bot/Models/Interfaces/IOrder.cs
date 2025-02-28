using System;

namespace Bot.Models.Interfaces
{
    public interface IOrder
    {
        public string OrderId { get; }

        public DateTime PlacementTime { get; }

        public DateTime ExecutionTime { get; }

        public string Symbol { get; }

        public decimal Quantity { get; }

        public decimal TargetPrice { get; }

        public decimal AverageFillPrice { get; }

        public OrderType Type { get; }

        public OrderState State { get; }
    }
}
