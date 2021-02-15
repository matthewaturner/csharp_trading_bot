using Bot.Brokerages;
using System.Collections.Generic;

namespace Bot.Interfaces.Trading
{
    public interface IStrategy
    {
        public List<string> OnTick(Dictionary<string, List<Tick>> stockData);
    }
}
