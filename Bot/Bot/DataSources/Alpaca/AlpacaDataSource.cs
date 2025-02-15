
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;
using Bot.Helpers;
using Bot.Models;

using Ap = Bot.DataSources.Alpaca.Models;

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
        var response = await _httpClient.GetAsync($"stocks/{symbol}/bars?timeframe={interval}&start={start.StdToString()}&end={end.StdToString()}");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var bars = JsonSerializer.Deserialize<Ap.BarsResponse>(json);
            return bars?.Bars.Select(b => b.ToBotModel(symbol)).ToList() ?? [];
        }
        else
        {
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        }

        throw new Exception($"API call failed: {response.StatusCode}");
    }
}