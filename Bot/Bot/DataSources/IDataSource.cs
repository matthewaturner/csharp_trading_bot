using Bot.Models;
using System.Threading.Tasks;
using System;
using Bot.Events;

namespace Bot.DataSources;

public interface IDataSource
{
    public event EventHandler<MarketDataEvent> MarketDataReceivers;

    /// <summary>
    /// Stream bars to the engine.
    /// </summary>
    /// <param name="symbols"></param>
    /// <param name="interval"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="callback"></param>
    /// <returns>Calls some callback for all bars.</returns>
    Task StreamBars(
        string[] symbols,
        Interval interval,
        DateTime start,
        DateTime? end);
}
