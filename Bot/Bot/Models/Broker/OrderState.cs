
using System.Runtime.Serialization;

namespace Bot.Models.Broker;

public enum OrderState
{
    [EnumMember(Value = "open")]
    Open,

    [EnumMember(Value = "cancelled")]
    Cancelled,

    [EnumMember(Value = "rejected")]
    Rejected,

    [EnumMember(Value = "filled")]
    Filled,

    /// <summary>
    /// Catch all for unimplemented broker-specific order states.
    /// </summary>
    [EnumMember(Value = "unknown")]
    Unknown

}
