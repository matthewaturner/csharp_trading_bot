namespace Bot.Models.Broker;

public class OrderRequest : IOrderRequest
{
    public OrderRequest(
        OrderType type,
        string symbol,
        double quantity)
    {
        Type = type;
        Symbol = symbol.ToUpper();
        Quantity = quantity;
    }

    public OrderType Type { get; set; }

    public string Symbol { get; set; }

    public double Quantity { get; set; }

    public static OrderRequest MarketBuy(string symbol, double quantity)
    {
        return new OrderRequest(OrderType.MarketBuy, symbol, quantity);
    }

    public static OrderRequest MarketSell(string symbol, double quantity)
    {
        return new OrderRequest(OrderType.MarketSell, symbol, quantity);
    }
}
