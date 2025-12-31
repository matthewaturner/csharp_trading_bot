
using Bot.Engine;
using Bot.Events;
using Bot.Helpers;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DataSources;

public abstract class DataSourceBase : IDataSource, IInitializeReceiver
{
    private MarketSnapshot CurrentSnapshot;
    private readonly string _dbPath;
    protected ILogger logger;
    protected ITradingEngine Engine { get; set; }

    public event EventHandler<MarketDataEvent> MarketDataReceivers;

    /// <summary>
    /// The name of the SQLite database file for caching (e.g., "alpaca.sqlite").
    /// </summary>
    protected abstract string CacheDatabaseName { get; }

    /// <summary>
    /// Handle initialize event.
    /// </summary>
    public void OnInitialize(object sender, EventArgs _)
    {
        Engine = sender as ITradingEngine;
        logger = Engine.CreateLogger(this.GetType().Name);
        logger.LogInformation("Initializing cache database at: {DbPath}", _dbPath);
    }

    protected DataSourceBase()
    {
        _dbPath = Path.Combine(GlobalConfig.DataFolder, CacheDatabaseName);
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS bars (
                symbol TEXT NOT NULL,
                interval TEXT NOT NULL,
                timestamp TEXT NOT NULL,
                open REAL NOT NULL,
                high REAL NOT NULL,
                low REAL NOT NULL,
                close REAL NOT NULL,
                adj_close REAL,
                volume INTEGER NOT NULL,
                PRIMARY KEY (symbol, interval, timestamp)
            )";
        command.ExecuteNonQuery();

        // Create index for faster lookups
        command.CommandText = @"
            CREATE INDEX IF NOT EXISTS idx_bars_lookup 
            ON bars(symbol, interval, timestamp)";
        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Stream bars to the engine.
    /// </summary>
    async Task IDataSource.StreamBars(
        List<string> symbols,
        Interval interval,
        DateTime start,
        DateTime? end)
    {
        end = end.HasValue ? end.Value : DateTime.UtcNow;

        Dictionary<string, List<Bar>> barsDict = new();

        foreach (string symbol in symbols)
        {
            var bars = await GetHistoricalBarsAsync(symbol, interval, start, end.Value);
            barsDict.Add(symbol, bars);
        }

        var snapshots = barsDict.SelectMany(kv => kv.Value)
            .Where(b => b.Timestamp >= start && b.Timestamp < end)
            .GroupBy(b => b.Timestamp)
            .OrderBy(g => g.Key)
            .Select(g => new MarketSnapshot(g.Key, g.ToArray()));

        foreach (MarketSnapshot snapshot in snapshots)
        {
            CurrentSnapshot = snapshot;
            MarketDataReceivers?.Invoke(this, new MarketDataEvent(snapshot));
        }
    }

    /// <summary>
    /// Get the latest bar for a given symbol.
    /// </summary>
    Bar IDataSource.GetLatestBar(string symbol)
    {
        return CurrentSnapshot[symbol];
    }

    /// <summary>
    /// Gets a list of bars for a symbol, interval, and period of time.
    /// This method implements read-through caching.
    /// </summary>
    public async Task<List<Bar>> GetHistoricalBarsAsync(
        string symbol,
        Interval interval,
        DateTime start,
        DateTime end)
    {
        logger?.LogDebug("GetHistoricalBarsAsync called: {Symbol} {Interval} from {Start} to {End}", 
            symbol, interval, start, end);
            
        // Try to get data from cache first
        var cachedBars = GetBarsFromCache(symbol, interval, start, end);
        
        // If we have all the data in cache, return it
        if (cachedBars.Count > 0)
        {
            // Normalize timestamps based on interval granularity for proper comparison
            var cachedStartNormalized = cachedBars.Min(b => b.Timestamp).NormalizeForInterval(interval);
            var cachedEndNormalized = cachedBars.Max(b => b.Timestamp).NormalizeForInterval(interval);
            var startNormalized = start.NormalizeForInterval(interval);
            var endNormalized = end.NormalizeForInterval(interval);
            
            // For daily intervals, adjust to nearest trading days
            // For intraday intervals, use the normalized timestamps directly
            DateTime firstValidPeriod, lastValidPeriod;
            if (interval.GetGranularityLevel() == 0)
            {
                // Daily or longer: adjust to trading days
                firstValidPeriod = startNormalized.GetNextTradingDayInclusive();
                lastValidPeriod = endNormalized.GetPreviousTradingDayInclusive();
                
                // If the requested end is today or in the future, check if today's data should be available
                var today = DateTime.UtcNow.Date;
                if (endNormalized >= today)
                {
                    // If it's 1+ hours after market close, expect today's data to be available
                    if (DateTimeHelper.IsAfterMarketCloseWithBuffer())
                    {
                        // Data for today should be available - expect cache to have it
                        lastValidPeriod = today.GetPreviousTradingDayInclusive();
                    }
                    else
                    {
                        // Too soon after market close or market hasn't closed yet - accept yesterday's data
                        lastValidPeriod = today.AddDays(-1).GetPreviousTradingDayInclusive();
                    }
                }
            }
            else
            {
                // Intraday: use normalized timestamps
                firstValidPeriod = startNormalized;
                lastValidPeriod = endNormalized;
            }
            
            logger?.LogDebug("Cache coverage check: cached [{CachedStart:yyyy-MM-dd HH:mm} to {CachedEnd:yyyy-MM-dd HH:mm}] vs requested [{FirstValid:yyyy-MM-dd HH:mm} to {LastValid:yyyy-MM-dd HH:mm}]",
                cachedStartNormalized, cachedEndNormalized, firstValidPeriod, lastValidPeriod);
            
            // Check if cache covers the required range
            if (cachedStartNormalized <= firstValidPeriod && cachedEndNormalized >= lastValidPeriod)
            {
                var result = cachedBars.Where(b => b.Timestamp >= start && b.Timestamp <= end)
                    .OrderBy(b => b.Timestamp)
                    .ToList();
                logger?.LogInformation("Cache HIT: {Symbol} {Interval} ({Start} to {End}) - {Count} bars",
                    symbol, interval, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"), result.Count);
                return result;
            }
        }

        // Cache miss or partial data - fetch from source
        logger?.LogInformation("Cache MISS: {Symbol} {Interval} ({Start} to {End}) - fetching from source",
            symbol, interval, start.ToString("yyyy-MM-dd"), end.ToString("yyyy-MM-dd"));
        var bars = await GetHistoricalBarsInternalAsync(symbol, interval, start, end);
        
        // Store in cache
        StoreBarsInCache(bars, interval);
        
        logger?.LogInformation("Fetched and cached {Count} bars for {Symbol}", bars.Count, symbol);
        
        return bars;
    }

    /// <summary>
    /// Internal method to fetch historical bars from the data source.
    /// Derived classes should implement this instead of GetHistoricalBarsAsync.
    /// </summary>
    protected abstract Task<List<Bar>> GetHistoricalBarsInternalAsync(
        string symbol,
        Interval interval,
        DateTime start,
        DateTime end);

    private List<Bar> GetBarsFromCache(string symbol, Interval interval, DateTime start, DateTime end)
    {
        var bars = new List<Bar>();

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        // First, let's check if we have any data for this symbol/interval at all
        var countCommand = connection.CreateCommand();
        countCommand.CommandText = @"
            SELECT COUNT(*) 
            FROM bars 
            WHERE symbol = $symbol AND interval = $interval";
        countCommand.Parameters.AddWithValue("$symbol", symbol);
        countCommand.Parameters.AddWithValue("$interval", interval.ToString());
        var totalCount = (long)countCommand.ExecuteScalar();
        
        logger?.LogDebug("Cache has {TotalCount} total bars for {Symbol} {Interval}", totalCount, symbol, interval);

        // Now query for the specific date range
        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT symbol, timestamp, open, high, low, close, adj_close, volume
            FROM bars
            WHERE symbol = $symbol 
              AND interval = $interval 
              AND timestamp >= $start 
              AND timestamp <= $end
            ORDER BY timestamp";
        
        command.Parameters.AddWithValue("$symbol", symbol);
        command.Parameters.AddWithValue("$interval", interval.ToString());
        command.Parameters.AddWithValue("$start", start.ToString("o"));
        command.Parameters.AddWithValue("$end", end.ToString("o"));

        logger?.LogDebug("Cache query: symbol={Symbol}, interval={Interval}, start={Start}, end={End}", 
            symbol, interval.ToString(), start.ToString("o"), end.ToString("o"));

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var bar = new Bar(
                DateTime.Parse(reader.GetString(1)),
                reader.GetString(0),
                reader.GetDouble(2),
                reader.GetDouble(3),
                reader.GetDouble(4),
                reader.GetDouble(5),
                reader.GetInt64(7),
                reader.IsDBNull(6) ? null : reader.GetDouble(6)
            );
            bars.Add(bar);
        }

        logger?.LogDebug("Cache query returned {Count} bars", bars.Count);

        return bars;
    }

    private void StoreBarsInCache(List<Bar> bars, Interval interval)
    {
        if (bars == null || bars.Count == 0)
            return;

        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        using var transaction = connection.BeginTransaction();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT OR REPLACE INTO bars 
            (symbol, interval, timestamp, open, high, low, close, adj_close, volume)
            VALUES ($symbol, $interval, $timestamp, $open, $high, $low, $close, $adj_close, $volume)";

        var symbolParam = command.Parameters.Add("$symbol", SqliteType.Text);
        var intervalParam = command.Parameters.Add("$interval", SqliteType.Text);
        var timestampParam = command.Parameters.Add("$timestamp", SqliteType.Text);
        var openParam = command.Parameters.Add("$open", SqliteType.Real);
        var highParam = command.Parameters.Add("$high", SqliteType.Real);
        var lowParam = command.Parameters.Add("$low", SqliteType.Real);
        var closeParam = command.Parameters.Add("$close", SqliteType.Real);
        var adjCloseParam = command.Parameters.Add("$adj_close", SqliteType.Real);
        var volumeParam = command.Parameters.Add("$volume", SqliteType.Integer);

        string intervalStr = interval.ToString();
        
        logger?.LogDebug("Storing {Count} bars in cache with interval={Interval}, first bar timestamp={First}, last bar timestamp={Last}",
            bars.Count, intervalStr, bars.First().Timestamp.ToString("o"), bars.Last().Timestamp.ToString("o"));

        foreach (var bar in bars)
        {
            symbolParam.Value = bar.Symbol;
            intervalParam.Value = intervalStr;
            timestampParam.Value = bar.Timestamp.ToString("o");
            openParam.Value = bar.Open;
            highParam.Value = bar.High;
            lowParam.Value = bar.Low;
            closeParam.Value = bar.Close;
            adjCloseParam.Value = bar.AdjClose;
            volumeParam.Value = bar.Volume;

            command.ExecuteNonQuery();
        }

        transaction.Commit();
    }
}
