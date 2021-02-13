using Bot.Configuration;
using Bot.DataCollection.Yahoo;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace Bot
{
    class Program
    {
        public static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            IServiceCollection services = new ServiceCollection();

            // inject singletons
            services.AddSingleton<HttpClient>();
            services.AddSingleton<YahooDataProvider>();

            // setup configurations
            IConfiguration configuration = new ConfigurationBuilder()
                .Add(source: new JsonConfigurationSource()
                {
                    Path = "./appsettings.dev.json"
                })
                .Build();
            services.Configure<YahooDataConfiguration>(configuration.GetSection(ConfigurationPaths.Yahoo));

            IServiceProvider provider = services.BuildServiceProvider();

            YahooDataProvider yahoo = (YahooDataProvider)provider.GetService(typeof(YahooDataProvider));
            yahoo.GetCSV();
        }
    }
}
