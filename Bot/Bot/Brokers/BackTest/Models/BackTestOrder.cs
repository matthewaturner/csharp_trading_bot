
using Bot.Brokers.BackTest.Models;
using Bot.Models.Interfaces;
using System;

namespace Bot.Models
{
    public class BackTestOrder : IOrder
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="placementTime"></param>
        /// <param name="executionTime"></param>
        /// <param name="symbol"></param>
        /// <param name="executionPrice"></param>
        /// <param name="targetPrice"></param>
        /// <param name="quantity"></param>
        /// <param name="type"></param>
        /// <param name="state"></param>
        public BackTestOrder(
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
            AverageFillPrice = executionPrice;
            TargetPrice = targetPrice;
            Quantity = quantity;
            Type = type;
            State = state;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="request"></param>
        public BackTestOrder(IOrderRequest request)
        {
            OrderId = Guid.NewGuid().ToString();
            PlacementTime = DateTime.Now;
            ExecutionTime = new DateTime();
            Symbol = request.Symbol.ToUpper();
            AverageFillPrice = double.NaN;
            TargetPrice = request.TargetPrice;
            Quantity = request.Quantity;
            Type = request.Type;
            State = OrderState.Open;
        }

        /// <summary>
        /// Unique id of the order.
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// Time the order was placed.
        /// </summary>
        public DateTime PlacementTime { get; set; }

        /// <summary>
        /// Time the order was executed.
        /// </summary>
        public DateTime ExecutionTime { get; set; }

        /// <summary>
        /// Symbol the order is for.
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// Price the order actually executed at.
        /// </summary>
        public double AverageFillPrice { get; set; }

        /// <summary>
        /// Price the order is for at placement time.
        /// Best bid/ask for market orders, limit price for limit orders.
        /// </summary>
        public double TargetPrice { get; set; }

        /// <summary>
        /// Units to buy or sell.
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Type of the order. Market buy, etc.
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// State of the order. Open, filled, etc.
        /// </summary>
        public OrderState State { get; set; }

        /// <summary>
        /// Updates the order with fill information.
        /// </summary>
        /// <param name="price"></param>
        /// <param name="executionTime"></param>
        public void Fill(double price, DateTime executionTime)
        {
            AverageFillPrice = price;
            State = OrderState.Filled;
            ExecutionTime = executionTime;
        }

        /// <summary>
        /// Prints order to the string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{{Symbol:{Symbol}, Quantity:{Quantity} Type:{Type}, State:{State}, Price:{AverageFillPrice}}}";
        }
    }
}
