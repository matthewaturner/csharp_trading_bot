// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Broker;
using System;

namespace Bot.Brokers;

/// <summary>
/// Simpler syntax for common broker operations.
/// </summary>
public static class BrokerExtensions
{
    /// <summary>
    /// Shorthand method for market buy.
    /// </summary>
    public static IOrder MarketBuy(this IBroker broker, string symbol, double quantity)
    {
        return broker.PlaceOrder(OrderRequest.MarketBuy(symbol, quantity));
    }

    /// <summary>
    /// Shorthand method for market sell.
    /// </summary>
    public static IOrder MarketSell(this IBroker broker, string symbol, double quantity)
    {
        return broker.PlaceOrder(OrderRequest.MarketSell(symbol, quantity));
    }

    /// <summary>
    /// Closes a position in a given symbol.
    /// </summary>
    public static IOrder ClosePosition(this IBroker broker, string symbol)
    {
        var position = broker.GetPosition(symbol);
        OrderRequest order = new OrderRequest(
            position.Quantity > 0 ? OrderType.MarketSell : OrderType.MarketBuy,
            symbol,
            Math.Abs(position.Quantity));
        return broker.PlaceOrder(order);
    }
}
