using Bot.Configuration;
using Bot.DataCollection.Yahoo;
using Bot.DataStorage;
using Bot.DataStorage.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.Net.Http;

namespace Bot
{
    class Program
    {
        public static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            IServiceCollection services = new ServiceCollection();

            //for Connection
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
            var factory = DbProviderFactories.GetFactory("System.Data.SqlClient");
            DbConnection connection = factory.CreateConnection();

            // inject singletons
            services.AddSingleton<HttpClient>();
            services.AddSingleton<YahooDataProvider>();
            services.AddSingleton<TickStorage>();

            // setup configurations
            IConfiguration configuration = new ConfigurationBuilder()
                .Add(source: new JsonConfigurationSource()
                {
                    Path = "./appsettings.dev.json"
                })
                .Build();
            services.Configure<YahooDataConfiguration>(configuration.GetSection(ConfigurationPaths.Yahoo));
            services.Configure<SqlConfiguration>(configuration.GetSection(ConfigurationPaths.Sql));
            services.Configure<PriceContext>(configuration.GetSection(ConfigurationPaths.Sql));

            IServiceProvider provider = services.BuildServiceProvider();

            YahooDataProvider yahoo = (YahooDataProvider)provider.GetService(typeof(YahooDataProvider));
            TickStorage tickStorage = (TickStorage)provider.GetService(typeof(TickStorage));

            tickStorage.SaveTick(new Tick("test", DateTime.Now, TickLength.Day, 1, 2, 3, 4, 5, 6));
        }
    }
}
