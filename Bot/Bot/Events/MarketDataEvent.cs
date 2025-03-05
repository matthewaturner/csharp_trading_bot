using Bot.Models.MarketData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Events;

/// <summary>
/// An event containing new market data.
/// </summary>
/// <param name="snapshot"></param>
public class MarketDataEvent(MarketSnapshot snapshot)
{
    public MarketSnapshot Snapshot { get; } = snapshot;
}

