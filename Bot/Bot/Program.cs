using Bot.Configuration;
using Bot.Strategies;
using Bot.Engine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.Net.Http;
using Bot.Analyzers;
using Bot.Data;
using System.IO;
using Bot.Brokers;
using Bot.Brokers.BackTest;
using Newtonsoft.Json;
using Bot.Analyzers.Loggers;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
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
            Console.WriteLine("Running!");
            IServiceCollection services = new ServiceCollection();

            // setup configurations
            IConfiguration configuration = new ConfigurationBuilder()
                .Add(source: new JsonConfigurationSource()
                {
                    Path = "./appsettings.json"
                })
                .Add(source: new EnvironmentVariablesConfigurationSource())
                .Build();
            services.Configure<YahooDataConfiguration>(configuration.GetSection(ConfigurationPaths.Yahoo));
            services.Configure<AlpacaConfiguration>(configuration.GetSection(ConfigurationPaths.Alpaca));

            // entity framework stuff
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);

            // inject platform level things
            services.AddSingleton<HttpClient>();

            // inject data sources
            services.AddSingleton<YahooDataSource>();

            // inject brokers
            services.AddSingleton<BackTestingBroker>();
            services.AddSingleton<AlpacaBroker>();

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
                    case nameof(AlpacaBroker):
                        return serviceProvider.GetService<AlpacaBroker>();
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

            // read config file
            string configString = null;
            if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
            {
                configString = File.ReadAllText(args[0]);
            }
            else
            {
                configString = GetDefaultConfig();
            }
            var engineConfig = JsonConvert.DeserializeObject<EngineConfig>(configString);
            
            // write config file to the output directory for future reference
            string outputPath = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                $"CSharpTradeBot/{engineConfig.Strategy.Name}.{DateTimeOffset.Now.ToUnixTimeSeconds()}");
            string configOutputPath = Path.Join(outputPath, "engineConfig.json");
            Directory.CreateDirectory(outputPath);
            File.WriteAllText(configOutputPath, configString);

            engine.Initialize(engineConfig, outputPath);
            engine.RunAsync().Wait();
        }

        /// <summary>
        /// Gets default config for test purposes.
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultConfig()
        {
            var config = new EngineConfig()
            {
                Symbols = new List<string>() { "MSFT" },
                Interval = Models.TickInterval.Day,
                Start = new DateTime(2010, 1, 1),
                End = new DateTime(2020, 1, 1),
                DataSource = new DependencyConfig()
                {
                    Name = "YahooDataSource",
                    Args = null
                },
                Broker = new DependencyConfig()
                { 
                    Name = "BackTestingBroker",
                    Args = new string[] { "10000" }
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
                        Name = "ConsoleLogger",
                        Args = null
                    },
                    new DependencyConfig()
                    {
                        Name = "SharpeRatio",
                        Args = new string[] { "0.00005357" }
                    }
                }
            };
            return JsonConvert.SerializeObject(config);
        }
    }
}
