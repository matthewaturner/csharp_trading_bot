using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Bot.Configuration
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RunMode
    {
        [EnumMember(Value = "backtest")]
        BackTest,

        [EnumMember(Value = "paper")]
        Paper,

        [EnumMember(Value = "live")]
        Live
    }
}
