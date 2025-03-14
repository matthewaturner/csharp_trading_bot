﻿// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Bot.Brokers.Alpaca.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum AlpacaOrderType
{
    [EnumMember(Value = "market")]
    Market,

    [EnumMember(Value = "limit")]
    Limit,

    [EnumMember(Value = "stop")]
    Stop,

    [EnumMember(Value = "stop_limit")]
    StopLimit,

    [EnumMember(Value = "trailing_stop")]
    TrailingStop
}
