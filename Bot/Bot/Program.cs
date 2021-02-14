using Bot.Configuration;
using Bot.DataCollection;
using Bot.DataStorage;
using Bot.DataStorage.Models;
using Core;
using Core.Configuration;
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
            IServiceCollection services = new ServiceCollection();

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
            services.Configure<KeyVaultConfiguration>(configuration.GetSection(ConfigurationPaths.KeyVault));

            // entity framework stuff
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);

            // inject singletons
            services.AddSingleton<IKeyVaultManager, KeyVaultManager>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IDataSource, YahooDataSource>();
            services.AddSingleton<ITickStorage, TickStorage>();

            IServiceProvider provider = services.BuildServiceProvider();

            //YahooDataSource yahoo = (YahooDataSource)provider.GetService(typeof(YahooDataSource));
            ITickStorage tickStorage = (ITickStorage)provider.GetService(typeof(ITickStorage));

            var ticks = tickStorage.GetTicksAsync("MSFT", TickInterval.Day, new DateTime(2010, 1, 1), new DateTime(2011, 1, 1)).Result;

            foreach (Tick t in ticks)
            {
                Console.WriteLine(t.ToString());
            }
        }
    }
}
