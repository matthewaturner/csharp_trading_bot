using Bot.Models;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public interface IStrategy
    {
        public void OnTick(IList<Tick> ticks);
    }
}
