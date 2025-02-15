using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Bot.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataInterval
    {
        Day = 0,

        Hour = 1,

        Minute = 2,
    }
}
