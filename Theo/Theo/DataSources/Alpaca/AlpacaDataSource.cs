
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Text.Json;

using Ap = Theo.DataSources.Alpaca.Models;

namespace Theo.DataSources.Alpaca;

public class AlpacaDataSource
{
    private readonly HttpClient _httpClient;

    public AlpacaDataSource(IConfiguration config)
    {
        string apiKeyId = config["Alpaca.ApiKeyId"];
        string apiKeySecret = config["Alpaca.ApiKeySecret"];

        _httpClient = new HttpClient() { BaseAddress = new Uri("https://data.alpaca.markets/v2/") };
        _httpClient.DefaultRequestHeaders.Add("APCA-API-KEY-ID", apiKeyId);
        _httpClient.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", apiKeySecret);
    }

    public async Task<List<Ap.Bar>> GetHistoricalBarsAsync(string symbol, string timeframe, string start, string end)
    {
        var response = await _httpClient.GetAsync($"stocks/{symbol}/bars?timeframe={timeframe}&start={start}&end={end}");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var bars = JsonSerializer.Deserialize<Ap.BarsResponse>(json);
            return bars?.Bars ?? [];
        }

        throw new Exception($"API call failed: {response.StatusCode}");
    }
}