
using Bot.Models;
using Bot.Models.Interfaces;

namespace Bot.Brokers;

/// <summary>
/// Simpler syntax for common broker operations.
/// </summary>
public static class BrokerExtensions
{
    public static IOrder MarketBuy(this IBroker broker, string symbol, decimal quantity)
    {
        return broker.PlaceOrder(OrderRequest.MarketBuy(symbol, quantity));
    }
}
