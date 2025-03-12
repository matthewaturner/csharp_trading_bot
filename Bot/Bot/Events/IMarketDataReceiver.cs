// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

namespace Bot.Events;

/// <summary>
/// Defines the method implemented by event receivers.
/// </summary>
public interface IMarketDataReceiver
{
    public void OnMarketData(object sender, MarketDataEvent e);
}

