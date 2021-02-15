using Bot.Indicators;
using Bot.Models;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public interface IStrategy
    {
        int Lookback { get; }

        bool Hydrated { get; }

        void OnTick(Tick ticks);
    }
}
