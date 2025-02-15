
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Text.Json;

using Ap = Bot.DataSources.Alpaca.Models;

namespace Bot.DataSources.Alpaca;

public class AlpacaDataSource
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

    public async Task<List<Ap.Bar>> GetHistoricalBarsAsync(string symbol, string timeframe, string start, string end)
    {
        var response = await _httpClient.GetAsync($"stocks/{symbol}/bars?timeframe={timeframe}&start={start}&end={end}");

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var bars = JsonSerializer.Deserialize<Ap.BarsResponse>(json);
            return bars?.Bars ?? [];
        }
        else
        {
            Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        }

        throw new Exception($"API call failed: {response.StatusCode}");
    }
}