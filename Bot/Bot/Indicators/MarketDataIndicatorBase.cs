// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Events;
using Bot.Models.MarketData;

namespace Bot.Indicators;

public abstract class MarketDataIndicatorBase<T_out>(int lookback) : IIndicator<MarketSnapshot, T_out>, IMarketDataReceiver
{
    private int _count = 0;
    protected int _lookback = lookback;

    public void OnMarketData(object sender, MarketDataEvent e)
    {
        this.Iterate(e.Snapshot);
        _count++;
    }

    public bool IsHydrated => _count >= _lookback;

    // Abstract things

    protected abstract T_out Iterate(MarketSnapshot input);


    public abstract T_out Value { get; }
}
