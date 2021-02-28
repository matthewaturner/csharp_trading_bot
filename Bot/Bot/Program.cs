using Bot.Configuration;
using Bot.DataCollection;
using Bot.DataStorage;
using Bot.DataStorage.Models;
using Bot.Models;
using Bot.Strategies;
using Bot.Engine;
using Core;
using Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.Net.Http;
using Bot.Analyzers;
using System.Collections.Generic;

namespace Bot
{
    public class Program
    {
        public delegate IDataSource DataSourceResolver(string key);

        public delegate IBroker BrokerResolver(string key);

        public delegate IStrategy StrategyResolver(string key);

        public delegate IAnalyzer AnalyzerResolver(string key);

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

            // inject platform level things
            services.AddSingleton<IKeyVaultManager, KeyVaultManager>();
            //services.AddSingleton<ITickStorage, TickStorage>();
            services.AddSingleton<HttpClient>();

            // inject data sources
            services.AddSingleton<YahooDataSource>();

            // inject brokers
            services.AddSingleton<BackTestingBroker>();

            // inject strategies
            services.AddSingleton<SMACrossoverStrategy>();

            // inject analyzers
            services.AddSingleton<ConsoleLogger>();
            services.AddSingleton<SharpeRatio>();

            var serviceProvider = services.BuildServiceProvider();

            // resolvers do named lookup on singleton services
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

            services.AddSingleton<AnalyzerResolver>(serviceProvider => key =>
            {
                switch (key)
                {
                    case nameof(ConsoleLogger):
                        return serviceProvider.GetService<ConsoleLogger>();
                    case nameof(SharpeRatio):
                        return serviceProvider.GetService<SharpeRatio>();
                    default:
                        return null;
                }
            });

            services.AddSingleton<ITradingEngine, TradingEngine>();
            serviceProvider = services.BuildServiceProvider();

            ITradingEngine engine = serviceProvider.GetService<ITradingEngine>();

            var engineConfig = new EngineConfig()
            {
                Symbols = new List<string>() { "MSFT" },
                Interval = TickInterval.Day,
                Start = new DateTime(2010, 1, 1),
                End = new DateTime(2021, 1, 1),
                DataSource = new DependencyConfig()
                {
                    Name = "YahooDataSource"
                },
                Broker = new DependencyConfig()
                {
                    Name = "BackTestingBroker",
                    Args = new string[] { "1000" }
                },
                Strategy = new DependencyConfig()
                {
                    Name = "SMACrossoverStrategy",
                    Args = new string[] { "MSFT", "16", "64", "true" }
                },
                Analyzers = new List<DependencyConfig>()
                {
                    new DependencyConfig()
                    {
                        Name = "SharpeRatio",
                        Args = new string[] { "0.00005357"}
                    },
                }

            };
            engine.RunAsync(engineConfig).Wait();

            engine.RunAsync(engineConfig).Wait();
            //engine.RunAsync("engineConfig.json").Wait();
        }
    }
}
