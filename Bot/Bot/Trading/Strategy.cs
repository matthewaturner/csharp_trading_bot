using Bot.Brokerages;
using Bot.Interfaces.Trading;
using System.Collections.Generic;

namespace Bot.Trading
{
    public class AlwaysBuy_Strategy : IStrategy
    {
        public List<string> OnTick(Dictionary<string, List<Tick>> stockData)
        {
            return new List<string>(stockData.Keys);
        }
    }
}
