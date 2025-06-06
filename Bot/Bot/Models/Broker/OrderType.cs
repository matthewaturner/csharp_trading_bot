﻿// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Bot.Models.Broker;

[JsonConverter(typeof(StringEnumConverter))]
public enum OrderType
{
    [EnumMember(Value = "marketBuy")]
    MarketBuy,

    [EnumMember(Value = "marketSell")]
    MarketSell,

    /// <summary>
    /// For order types which are not implemented yet.
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown
}
