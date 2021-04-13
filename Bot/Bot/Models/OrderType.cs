
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Bot.Models
{
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
}
