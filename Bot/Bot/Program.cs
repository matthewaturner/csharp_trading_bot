using Bot.Configuration;
using Bot.DataCollection;
using Bot.DataStorage;
using Bot.DataStorage.Models;
using Bot.Engine;
using Bot.Strategies;
using Bot.Trading;
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
    public class Program
    {
        public delegate IDataSource DataSourceResolver(string key);

        public delegate IBroker BrokerResolver(string key);

        public delegate IStrategy StrategyResolver(string key);

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
            services.Configure<TickContext>(configuration.GetSection(ConfigurationPaths.Sql));
            services.Configure<KeyVaultConfiguration>(configuration.GetSection(ConfigurationPaths.KeyVault));

            // entity framework stuff
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);

            // inject singletons
            services.AddSingleton<IKeyVaultManager, KeyVaultManager>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<ITickStorage, TickStorage>();
            services.AddSingleton<ITicks, Ticks>();

            // ticks object is special
            Ticks ticks = new Ticks();
            services.AddSingleton<ITicks>(ticks);
            services.AddSingleton(ticks);

            // inject named dependency resolvers
            services.AddSingleton<YahooDataSource>();
            services.AddSingleton<BackTestingBroker>();
            services.AddSingleton<SMACrossoverStrategy>();

            var serviceProvider = services.BuildServiceProvider();

            services.AddSingleton<DataSourceResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case nameof(YahooDataSource):
                        return serviceProvider.GetService<YahooDataSource>();
                    default:
                        return null;
                }
            });

            services.AddSingleton<BrokerResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case nameof(BackTestingBroker):
                        return serviceProvider.GetService<BackTestingBroker>();
                    default:
                        return null;
                }
            });

            services.AddSingleton<StrategyResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case nameof(SMACrossoverStrategy):
                        return serviceProvider.GetService<SMACrossoverStrategy>();
                    default:
                        return null;
                }
            });

            services.AddSingleton<ITradingEngine, TradingEngine>();
            serviceProvider = services.BuildServiceProvider();

            ITradingEngine engine = serviceProvider.GetService<ITradingEngine>();
            engine.RunAsync().Wait();
        }
    }
}
