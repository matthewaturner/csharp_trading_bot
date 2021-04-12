
using System.Runtime.Serialization;

namespace Bot.Brokers.Alpaca.Models
{
    public enum AlpacaOrderSide
    {
        [EnumMember(Value = "buy")]
        Buy,

        [EnumMember(Value = "sell")]
        Sell
    }
}
