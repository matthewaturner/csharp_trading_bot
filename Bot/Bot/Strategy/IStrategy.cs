
using Bot.Indicators;
using Bot.Models;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public interface IStrategy
    {
        int Lookback { get; }

        bool IsHydrated { get; }

        IList<IIndicator> Indicators { get; }

        void ProcessBar(Bar bar);
    }
}
