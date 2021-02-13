using Bot.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Bot.DataCollection.Yahoo
{
    public class YahooDataProvider
    {
        YahooDataConfiguration config;
        HttpClient httpClient;

        public YahooDataProvider(
            IOptionsSnapshot<YahooDataConfiguration> config,
            HttpClient httpClient)
        {
            this.config = config.Value;
            this.httpClient = httpClient;
        }

        public void GetCSV()
        {
            Console.WriteLine("Called GetCSV!");
            Console.WriteLine($"BaseURL: {config.BaseUrl}");
        }
    }
}
