using Bot.Models.Broker;
using Bot.Models.Interfaces;

namespace Bot.Brokers;

/// <summary>
/// Simpler syntax for common broker operations.
/// </summary>
public static class BrokerExtensions
{
    public static IOrder MarketBuy(this IBroker broker, string symbol, double quantity)
    {
        return broker.PlaceOrder(OrderRequest.MarketBuy(symbol, quantity));
    }

    public static IOrder MarketSell(this IBroker broker, string symbol, double quantity)
    {
        return broker.PlaceOrder(OrderRequest.MarketSell(symbol, quantity));
    }
}
