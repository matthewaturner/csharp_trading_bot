using Theo.Models.Interfaces;

namespace Theo.Models
{
    public class OrderRequest : IOrderRequest
    {
        public OrderRequest(
            OrderType type,
            string symbol,
            double quantity,
            double targetPrice)
        {
            Type = type;
            Symbol = symbol.ToUpper();
            Quantity = quantity;
            TargetPrice = targetPrice;
        }

        public OrderType Type { get; set; }

        public string Symbol { get; set; }

        public double Quantity { get; set; }

        public double TargetPrice { get; set; }

    }
}
