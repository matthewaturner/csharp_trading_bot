﻿using Bot.Models.Interfaces;

namespace Bot.Models
{
    public class OrderRequest : IOrderRequest
    {
        public OrderRequest(
            OrderType type,
            string symbol,
            decimal quantity,
            decimal targetPrice)
        {
            Type = type;
            Symbol = symbol.ToUpper();
            Quantity = quantity;
            TargetPrice = targetPrice;
        }

        public OrderType Type { get; set; }

        public string Symbol { get; set; }

        public decimal Quantity { get; set; }

        public decimal TargetPrice { get; set; }

        public static OrderRequest MarketBuy(string symbol, decimal quantity)
        {
            return new OrderRequest(OrderType.MarketBuy, symbol, quantity, 0);
        }
    }
}
