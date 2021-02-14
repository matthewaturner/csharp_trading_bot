using Bot.DataStorage.Models;
using Bot.Interfaces.Trading;
using System;
using System.Collections.Generic;
using System.Text;

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
