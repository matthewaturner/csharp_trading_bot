using Bot.Configuration;
using Bot.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Bot.Engine;

namespace Bot.DataCollection
{
    public class YahooDataSource : IDataSource
    {
        const string urlFormat = "v7/finance/download/{0}?period1={1}&period2={2}&interval=1d&events=history&includeAdjustedClose=true";

        YahooDataConfiguration config;
        HttpClient httpClient;

        /// <summary>
        /// Constructs data source.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="httpClient"></param>
        public YahooDataSource(
            IOptionsSnapshot<YahooDataConfiguration> config,
            HttpClient httpClient)
        {
            this.config = config.Value;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Initializes the data source with custom arguments.
        /// </summary>
        public void Initialize(ITradingEngine engine, string[] args)
        { }

        /// <summary>
        /// Create request url for yahoo.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private string FormalRequestUrl(
            string symbol,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            return config.BaseUrl + string.Format(
                urlFormat, 
                symbol, 
                start.ToUnixTimeSeconds(), 
                end.ToUnixTimeSeconds());
        }

        /// <summary>
        /// Gets a stream of data.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<Stream> GetDataStream(
            string symbol, 
            TickInterval interval, 
            DateTimeOffset start, 
            DateTimeOffset end)
        {
            if (interval != TickInterval.Day)
            {
                throw new NotImplementedException();
            }

            try
            {
                //httpClient.BaseAddress = new Uri(config.BaseUrl);
                //httpClient.Timeout = TimeSpan.FromMinutes(5);
                string requestUrl = this.FormalRequestUrl(symbol, start, end);

                Console.WriteLine($"Downloading from {requestUrl}");

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStreamAsync();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error while downloading yahoo csv data for {symbol}. {ex}");
                return null;
            }
        }

        /// <summary>
        /// Gets a list of ticks.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<IList<Tick>> GetTicksAsync(string symbol, TickInterval interval, DateTime start, DateTime end)
        {
            Stream dataStream = await GetDataStream(symbol, interval, start, end);
            StreamReader reader = new StreamReader(dataStream);
            IList<Tick> tickList = new List<Tick>();

            // the first line is just headers
            string data = reader.ReadLine();

            while ((data = await reader.ReadLineAsync()) != null)
            {
                string[] tickStrings = data.Split(',', StringSplitOptions.None).Select(str => str.Trim()).ToArray();
                Tick tick;

                try
                {
                    DateTime dateTime = DateTime.ParseExact(tickStrings[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    double open     = Math.Round(double.Parse(tickStrings[1]), 4);
                    double high     = Math.Round(double.Parse(tickStrings[2]), 4);
                    double low      = Math.Round(double.Parse(tickStrings[3]), 4);
                    double close    = Math.Round(double.Parse(tickStrings[4]), 4);
                    double adjClose = Math.Round(double.Parse(tickStrings[5]), 4);
                    int volume      = int.Parse(tickStrings[6]);

                    tick = new Tick(symbol, interval, dateTime, open, high, low, close, adjClose, volume);
                    tickList.Add(tick);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Caught exception while parsing tick data. Data: '{data}' Exception: {ex}");
                    throw;
                }

            }

            return tickList;
        }


    }
}
