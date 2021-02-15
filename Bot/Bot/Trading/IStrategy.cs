using Bot.Brokerages;
using System.Collections.Generic;

namespace Bot.Interfaces.Trading
{
    public interface IStrategy
    {
        public void OnTick(Tick data);
    }
}
