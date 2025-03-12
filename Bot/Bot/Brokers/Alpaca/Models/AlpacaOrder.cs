// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Broker;
using Newtonsoft.Json;
using System;

namespace Bot.Brokers.Alpaca.Models;

public class AlpacaOrder : IOrder
{
    // alpaca fields

    [JsonProperty("id")]
    public string OrderId { get; set; }

    [JsonProperty("created_at")]
    public string CreatedTime { get; set; }

    [JsonProperty("submitted_at")]
    public string PlacementTime { get; set; }

    [JsonProperty("updated_at")]
    public string UpdatedTime { get; set; }

    [JsonProperty("filled_at")]
    public string FilledTime { get; set; }

    [JsonProperty("expired_at")]
    public string ExpiredTime { get; set; }

    [JsonProperty("failed_at")]
    public string FailedTime { get; set; }

    [JsonProperty("symbol")]
    public string Symbol { get; set; }

    [JsonProperty("qty")]
    public string Quantity { get; set; }

    [JsonProperty("filled_avg_price")]
    public string AverageFillPrice { get; set; }

    [JsonProperty("type")]
    public AlpacaOrderType Type { get; set; }

    [JsonProperty("side")]
    public AlpacaOrderSide Side { get; set; }

    [JsonProperty("status")]
    public AlpacaOrderStatus Status { get; set; }

    // convert to IOrder interface

    [JsonIgnore]
    public DateTime ExecutionTime => DateTime.Parse(FilledTime);

    [JsonIgnore]
    public double TargetPrice => 0;

    [JsonIgnore]
    public OrderState State
    {
        get
        {
            switch (Status)
            {
                case AlpacaOrderStatus.New:
                case AlpacaOrderStatus.Accepted:
                case AlpacaOrderStatus.DoneForDay:
                case AlpacaOrderStatus.PartiallyFilled:
                case AlpacaOrderStatus.PendingNew:
                case AlpacaOrderStatus.AcceptedForBidding:
                    return OrderState.Open;

                case AlpacaOrderStatus.Filled:
                    return OrderState.Filled;

                case AlpacaOrderStatus.Rejected:
                    return OrderState.Rejected;

                case AlpacaOrderStatus.Cancelled:
                    return OrderState.Cancelled;

                default:
                    return OrderState.Unknown;
            }
        }
    }

    [JsonIgnore]
    DateTime IOrder.PlacementTime => DateTime.Parse(PlacementTime);

    [JsonIgnore]
    double IOrder.Quantity => double.Parse(Quantity);

    [JsonIgnore]
    double IOrder.AverageFillPrice => double.Parse(AverageFillPrice);

    [JsonIgnore]
    OrderType IOrder.Type
    {
        get
        {
            switch (Type, Side)
            {
                case (AlpacaOrderType.Market, AlpacaOrderSide.Buy):
                    return OrderType.MarketBuy;

                case (AlpacaOrderType.Market, AlpacaOrderSide.Sell):
                    return OrderType.MarketSell;

                default:
                    return OrderType.Unknown;

            }

        }
    }

}
