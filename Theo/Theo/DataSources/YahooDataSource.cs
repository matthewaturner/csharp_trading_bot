using Theo.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Theo.Data
{
    public class YahooDataSource : DataSourceBase
    {
        const string yahooBaseUrl = "https://query1.finance.yahoo.com/";
        const string urlFormat = "v7/finance/download/{0}?period1={1}&period2={2}&interval=1d&events=history&includeAdjustedClose=true";

        private HttpClient httpClient;

        /// <summary>
        /// Constructs data source.
        /// </summary>
        /// <param name="config"></param>
        /// <param name="httpClient"></param>
        public YahooDataSource(HttpClient httpClient = null)
        {
            this.httpClient = httpClient ?? new HttpClient();
        }

        /// <summary>
        /// Create request url for yahoo.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private string FormRequestUrl(
            string symbol,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            return yahooBaseUrl + string.Format(
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
            BarInterval interval,
            DateTimeOffset start,
            DateTimeOffset end)
        {
            if (interval != BarInterval.Day)
            {
                throw new NotImplementedException();
            }

            try
            {
                string requestUrl = FormRequestUrl(symbol, start, end);
                Engine.Logger.LogInformation($"Downloading from {requestUrl}");

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
        /// Gets a list of bars.
        /// </summary>
        /// <param name="symbol">Symbol to get bars for.</param>
        /// <param name="interval">Interval we are interested in.</param>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        /// <returns>List of bar objects.</returns>
        public override async Task<IList<Bar>> GetHistoricalBarsAsync(
            string symbol, 
            BarInterval interval, 
            DateTime start, 
            DateTime end)
        {
            Stream dataStream = await GetDataStream(symbol, interval, start, end);
            StreamReader reader = new StreamReader(dataStream);
            IList<Bar> barList = new List<Bar>();

            // the first line is just headers
            string data = reader.ReadLine();

            while ((data = await reader.ReadLineAsync()) != null)
            {
                string[] barStrings = data.Split(',', StringSplitOptions.None).Select(str => str.Trim()).ToArray();
                Bar bar;

                DateTime dateTime = DateTime.ParseExact(barStrings[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                double open = Math.Round(double.Parse(barStrings[1]), 4);
                double high = Math.Round(double.Parse(barStrings[2]), 4);
                double low = Math.Round(double.Parse(barStrings[3]), 4);
                double close = Math.Round(double.Parse(barStrings[4]), 4);
                double adjClose = Math.Round(double.Parse(barStrings[5]), 4);
                int volume = int.Parse(barStrings[6]);

                double adjustmentRatio = adjClose / close;
                double adjOpen = adjustmentRatio * open;
                double adjHigh = adjustmentRatio * high;
                double adjLow = adjustmentRatio * low;

                // volume can't be adjusted properly with current information
                bar = new Bar(symbol, interval, dateTime, adjOpen, adjHigh, adjLow, adjClose, volume);
                barList.Add(bar);
            }

            return barList;
        }
    }
}
