
using Bot.Engine;
using Bot.Engine.Events;
using Bot.Indicators;
using Bot.Models;
using System.Collections.Generic;

namespace Bot.Strategies
{
    public interface IStrategy
    {
        int Lookback { get; }

        bool Hydrated { get; }

        IList<IIndicator> Indicators { get; }

        void Initialize(ITradingEngine engine, string[] args);

        void StrategyOnTick(IMultiBar ticks);

        string[] GetCsvHeaders();

        string[] GetCsvValues();
    }
}
