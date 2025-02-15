
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Bot.Helpers;
using Bot.Models;

using Ap = Bot.DataSources.Alpaca.Models;
using System.CodeDom.Compiler;
using System.Collections.Specialized;
using System.Web;

namespace Bot.DataSources.Alpaca;

public class AlpacaDataSource : DataSourceBase
{
    private readonly HttpClient _httpClient;

    public AlpacaDataSource()
    {
        string apiKeyId = GlobalConfig.GetValue("Alpaca:KeyId");
        string apiKeySecret = GlobalConfig.GetValue("Alpaca:KeyIdSecret");

        _httpClient = new HttpClient() { BaseAddress = new Uri("https://data.alpaca.markets/v2/") };
        _httpClient.DefaultRequestHeaders.Add("APCA-API-KEY-ID", apiKeyId);
        _httpClient.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", apiKeySecret);
    }

    /// <summary>
    /// Get historical bar data.
    /// </summary>
    public override async Task<List<Bar>> GetHistoricalBarsAsync(string symbol, Interval interval, DateTime start, DateTime end)
    {
        var results = new List<Bar>();
        string? nextPageToken = null;

        do
        {
            var response = await _httpClient.GetAsync($"stocks/{symbol}/bars?" +
                $"timeframe={interval}&start={start.StdToString()}&end={end.StdToString()}&" +
                $"page_token={nextPageToken}");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var barsResult = JsonSerializer.Deserialize<Ap.BarsResponse>(json) 
                ?? throw new JsonException($"{nameof(GetHistoricalBarsAsync)}: Failed to deserialize response.");

            results.AddRange(barsResult?.Bars.Select(b => b.ToBotModel(symbol)));
            nextPageToken = barsResult.NextPageToken;
        }
        while (!string.IsNullOrEmpty(nextPageToken));

        return results;
    }
}