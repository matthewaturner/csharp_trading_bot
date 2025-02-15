
using Bot.Engine;
using Bot.Engine.Events;
using Bot.Indicators;
using Bot.Models;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public interface IStrategy : IBarReceiver
    {
        int Lookback { get; }

        bool IsHydrated { get; }

        IList<IIndicator> Indicators { get; }

        void Initialize(ITradingEngine engine);

        void OnBar(MultiBar bars);
    }
}
