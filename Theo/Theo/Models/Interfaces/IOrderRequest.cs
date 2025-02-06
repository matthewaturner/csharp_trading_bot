using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Theo.Models.Interfaces
{
    public interface IOrderRequest
    {
        /// <summary>
        /// Type of the order to place.
        /// </summary>
        public OrderType Type { get; }

        /// <summary>
        /// Symbol the order is for.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Quantity to buy or sell.
        /// </summary>
        public double Quantity { get; }

        /// <summary>
        /// Price when the order was placed. Useful to calculate slippage.
        /// </summary>
        public double TargetPrice { get; }
    }
}
