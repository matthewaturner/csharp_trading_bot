using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bot.Models
{
    public enum PositionType
    {
        [EnumMember(Value = "long")]
        Long,

        [EnumMember(Value = "short")]
        Short
    }
}
