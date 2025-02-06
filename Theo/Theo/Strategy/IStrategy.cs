
using Theo.Engine;
using Theo.Engine.Events;
using Theo.Indicators;
using Theo.Models;
using System.Collections.Generic;

namespace Theo.Strategies
{
    public interface IStrategy : ITickReceiver
    {
        int Lookback { get; }

        bool IsHydrated { get; }

        IList<IIndicator> Indicators { get; }

        void Initialize(ITradingEngine engine);

        void OnTick(IMultiTick ticks);
    }
}
