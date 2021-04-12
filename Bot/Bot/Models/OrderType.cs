
using System.Runtime.Serialization;
/// <summary>
/// Enum for trade type.
/// </summary>
namespace Bot.Models
{
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
}
