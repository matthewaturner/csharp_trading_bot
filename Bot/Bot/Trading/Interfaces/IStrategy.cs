using Bot.DataStorage.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Interfaces.Trading
{
    public interface IStrategy
    {
        public List<string> OnTick(Dictionary<string, List<Tick>> stockData);
    }
}
