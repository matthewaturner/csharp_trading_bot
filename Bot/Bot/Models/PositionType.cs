using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Bot.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PositionType
    {
        [EnumMember(Value = "long")]
        Long,

        [EnumMember(Value = "short")]
        Short,

        [EnumMember(Value = "neutral")]
        Neutral,
    }
}
