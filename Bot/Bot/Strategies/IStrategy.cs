
using Bot.Indicators;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public interface IStrategy
    {
        int Lookback { get; }

        bool Hydrated { get; }

        IList<IIndicator> Indicators { get; }

        void Initialize(string[] args);

        void OnTick();

    }
}
