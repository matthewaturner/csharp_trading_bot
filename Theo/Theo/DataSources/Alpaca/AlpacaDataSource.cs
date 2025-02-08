
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Theo.Models;
using System.Net.Http;
using System.Text.Json;
using Theo.DataSources.Alpaca.Models;

namespace Theo.DataSources.Alpaca;

public class AlpacaDataSource
{
    private readonly HttpClient _httpClient;

    public AlpacaDataSource(IConfiguration config)
    {
        string apiKeyId = config["Alpaca.ApiKeyId"];
        string apiKeySecret = config["Alpaca.ApiKeySecret"];

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://data.alpaca.markets/v2/");
        _httpClient.DefaultRequestHeaders.Add("APCA-API-KEY-ID", apiKeyId);
        _httpClient.DefaultRequestHeaders.Add("APCA-API-SECRET-KEY", apiKeySecret);
    }

    public async Task<List<Models.Bar>> GetHistoricalBarsAsync(string symbol, string timeframe, string start, string end)
    {
        var response = await _httpClient.GetAsync($"stocks/{symbol}/bars?timeframe={timeframe}&start={start}&end={end}");
        
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var bars = JsonSerializer.Deserialize<BarsResponse>(json);
            return bars?.Bars ?? new List<Models.Bar>();
        }

        throw new Exception($"API call failed: {response.StatusCode}");
    }
}