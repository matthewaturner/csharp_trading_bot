﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Theo.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum DataInterval
    {
        Day = 0,

        Hour = 1,

        Minute = 2,
    }
}
