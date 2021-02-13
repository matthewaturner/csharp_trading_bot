using Bot.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Bot.DataCollection.Yahoo
{
    public class YahooDataProvider
    {
        const string urlFormat = "v7/finance/download/{0}?period1={1}&period2={2}&interval=1d&events=history&includeAdjustedClose=true";

        YahooDataConfiguration config;
        HttpClient httpClient;

        public YahooDataProvider(
            IOptionsSnapshot<YahooDataConfiguration> config,
            HttpClient httpClient)
        {
            this.config = config.Value;
            this.httpClient = httpClient;
        }

        private string FormalRequestUrl(
            string symbol,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            return string.Format(urlFormat, symbol, start.ToUnixTimeSeconds(), end.ToUnixTimeSeconds());
        }

        public async Task<Stream> GetDataStream(string symbol, DateTimeOffset start, DateTimeOffset end)
        {
            try
            {
                httpClient.BaseAddress = new Uri(config.BaseUrl);
                httpClient.Timeout = TimeSpan.FromMinutes(5);
                string requestUrl = this.FormalRequestUrl(symbol, start, end);

                Console.WriteLine($"Downloading from {requestUrl}");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error while downloading csv data for {symbol}. {ex}");
                return null;
            }
        }

        public async void ProcessDataStream(string symbol)
        {
        }

        public async void DownloadAllData(string symbol, DateTimeOffset start, DateTimeOffset end)
        {
            Stream dataStream = await this.GetDataStream(symbol, start, end);
            StreamReader reader = new StreamReader(dataStream);

            string[] tickString;
            string data = reader.ReadLine();

            while ((data = reader.ReadLine()) != null)
            {
                tickString = data.Split(',');
            }
        }
    }
}
