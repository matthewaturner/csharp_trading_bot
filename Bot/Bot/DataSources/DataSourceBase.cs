using Bot.Engine;
using Bot.Events;
using Bot.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DataSources;

public abstract class DataSourceBase : IDataSource
{
    private ILogger Logger => GlobalConfig.Logger;

    public event EventHandler<MarketDataEvent> MarketDataReceivers;

    /// <summary>
    /// Stream bars to the engine.
    /// </summary>
    /// <param name="symbols"></param>
    /// <param name="interval"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public async Task StreamBars(
        string[] symbols,
        Interval interval,
        DateTime start,
        DateTime? end)
    {
        IDictionary<string, IList<Bar>> allBars = new Dictionary<string, IList<Bar>>(symbols.Length);
        end = end.HasValue ? end.Value : DateTime.UtcNow;

        foreach (string s in symbols)
        {
            allBars[s] = await GetHistoricalBarsAsync(s, interval, start, end.Value);
        }

        int barCount = allBars.Values.First().Count();
        if (!allBars.Values.All(bars => bars.Count() == barCount))
        {
            throw new DataMisalignedException("Got a different number of bars for each symbol.");
        }

        IList<IEnumerator<Bar>> enumerators = allBars.Values.Select(bars => bars.GetEnumerator()).ToList();
        while (enumerators.All(e => e.MoveNext()) && enumerators[0].Current.Timestamp < end.Value)
        {
            foreach (var e in enumerators)
            {
                try
                {
                    MarketDataReceivers?.Invoke(this, new MarketDataEvent(e.Current));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, ex.ToString());
                }
            }
        }
    }

    /// <summary>
    /// Gets a list of bars for a symbol, interval, and period of time.
    /// </summary>
    public abstract Task<List<Bar>> GetHistoricalBarsAsync(
        string symbol, 
        Interval interval, 
        DateTime start, 
        DateTime end);
}
