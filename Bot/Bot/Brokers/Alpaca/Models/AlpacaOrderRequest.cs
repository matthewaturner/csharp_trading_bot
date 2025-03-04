using Bot.Models.Broker;
using Bot.Models.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Bot.Brokers.Alpaca.Models;

public class AlpacaOrderRequest
{
    public AlpacaOrderRequest(IOrderRequest request)
    {
        Symbol = request.Symbol;
        Quantity = request.Quantity.ToString();
        Notional = null;
        TimeInForce = "day";
        LimitPrice = null;
        StopPrice = null;
        TrailPrice = null;
        TrailPercent = null;
        ExtendedHours = null;
        ClientOrderId = null;
        OrderClass = null;
        TakeProfit = null;
        StopLoss = null;

        switch (request.Type)
        {
            case OrderType.MarketBuy:
                Side = AlpacaOrderSide.Buy;
                Type = AlpacaOrderType.Market;
                break;

            case OrderType.MarketSell:
                Side = AlpacaOrderSide.Sell;
                Type = AlpacaOrderType.Market;
                break;

            default:
                throw new NotImplementedException();
        }
    }

    [JsonProperty("symbol")]
    [JsonRequired]
    public string Symbol { get; set; }

    [JsonProperty("qty")]
    [JsonRequired]
    public string Quantity { get; set; }

    [JsonProperty("side")]
    [JsonRequired]
    [JsonConverter(typeof(StringEnumConverter))]
    public AlpacaOrderSide Side { get; set; }

    [JsonProperty("type")]
    [JsonRequired]
    [JsonConverter(typeof(StringEnumConverter))]
    public AlpacaOrderType Type { get; set; }

    [JsonProperty("time_in_force")]
    [JsonRequired]
    public string TimeInForce { get; set; }

    [JsonProperty("notional", NullValueHandling = NullValueHandling.Ignore)]
    public string Notional { get; set; }

    [JsonProperty("limit_price", NullValueHandling = NullValueHandling.Ignore)]
    public string LimitPrice { get; set; }

    [JsonProperty("stop_price", NullValueHandling = NullValueHandling.Ignore)]
    public string StopPrice { get; set; }

    [JsonProperty("trail_price", NullValueHandling = NullValueHandling.Ignore)]
    public string TrailPrice { get; set; }

    [JsonProperty("trail_percent", NullValueHandling = NullValueHandling.Ignore)]
    public string TrailPercent { get; set; }

    [JsonProperty("extended_hours", NullValueHandling = NullValueHandling.Ignore)]
    public bool? ExtendedHours { get; set; }

    [JsonProperty("client_order_id", NullValueHandling = NullValueHandling.Ignore)]
    public string ClientOrderId { get; set; }

    [JsonProperty("order_class", NullValueHandling = NullValueHandling.Ignore)]
    public string OrderClass { get; set; }

    [JsonProperty("take_profit", NullValueHandling = NullValueHandling.Ignore)]
    public object TakeProfit { get; set; }

    [JsonProperty("stop_loss", NullValueHandling = NullValueHandling.Ignore)]
    public object StopLoss { get; set; }
}
