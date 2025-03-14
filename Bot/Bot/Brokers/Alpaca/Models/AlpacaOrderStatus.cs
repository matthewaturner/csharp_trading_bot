﻿// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Bot.Brokers.Alpaca.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum AlpacaOrderStatus
{
    [EnumMember(Value = "new")]
    New,

    [EnumMember(Value = "partially_filled")]
    PartiallyFilled,

    [EnumMember(Value = "done_for_day")]
    DoneForDay,

    [EnumMember(Value = "filled")]
    Filled,

    [EnumMember(Value = "canceled")]
    Cancelled,

    [EnumMember(Value = "expired")]
    Expired,

    [EnumMember(Value = "replaced")]
    Replaced,

    [EnumMember(Value = "pending_cancel")]
    PendingCancel,

    [EnumMember(Value = "pending_replace")]
    PendingReplace,

    [EnumMember(Value = "accepted")]
    Accepted,

    [EnumMember(Value = "pending_new")]
    PendingNew,

    [EnumMember(Value = "accepted_for_bidding")]
    AcceptedForBidding,

    [EnumMember(Value = "stopped")]
    Stopped,

    [EnumMember(Value = "rejected")]
    Rejected,

    [EnumMember(Value = "suspended")]
    Suspended,

    [EnumMember(Value = "calculated")]
    Calculated

}
