
using Bot.Engine;
using Bot.Engine.Events;
using Bot.Indicators;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public interface IStrategy : ITickReceiver
    {
        int Lookback { get; }

        bool Hydrated { get; }

        IList<IIndicator> Indicators { get; }

        void Initialize(ITradingEngine engine, string[] args);
    }
}
