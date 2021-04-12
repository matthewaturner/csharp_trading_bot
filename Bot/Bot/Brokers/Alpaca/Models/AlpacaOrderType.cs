using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Brokers.Alpaca.Models
{
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
}
