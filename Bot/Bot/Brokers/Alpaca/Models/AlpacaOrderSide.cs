﻿
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Bot.Brokers.Alpaca.Models;


[JsonConverter(typeof(StringEnumConverter))]
public enum AlpacaOrderSide
{
    [EnumMember(Value = "buy")]
    Buy,

    [EnumMember(Value = "sell")]
    Sell
}
