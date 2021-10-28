using Bot.Configuration;
using Bot.Models;
using Bot.Strategies;
using Bot.Engine;
using Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using RDotNet;
using System;
using System.Data.Common;
using System.Net.Http;
using Bot.Analyzers;
using System.Collections.Generic;
using Bot.Data;
using System.IO;
using Bot.Brokers;
using Core.Azure;
using Bot.Brokers.BackTest;
using Newtonsoft.Json;
using Bot.Analyzers.Loggers;

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
                    Path = "./appsettings.dev.json"
                })
                .Build();
            services.Configure<KeyVaultConfiguration>(configuration.GetSection(ConfigurationPaths.KeyVault));
            services.Configure<YahooDataConfiguration>(configuration.GetSection(ConfigurationPaths.Yahoo));
            services.Configure<AlpacaConfiguration>(configuration.GetSection(ConfigurationPaths.Alpaca));

            // entity framework stuff
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);

            // inject platform level things
            services.AddSingleton<IKeyVaultManager, KeyVaultManager>();
            services.AddSingleton<HttpClient>();

            // inject data sources
            services.AddSingleton<YahooDataSource>();

            // inject brokers
            services.AddSingleton<BackTestingBroker>();
            services.AddSingleton<AlpacaBroker>();

            // inject strategies
            services.AddSingleton<SMACrossoverStrategy>();
            services.AddSingleton<BollingerMeanReversion>();

            // inject analyzers
            services.AddSingleton<ConsoleLogger>();
            services.AddSingleton<CsvLogger>();
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
                    case nameof(BollingerMeanReversion):
                        return serviceProvider.GetService<BollingerMeanReversion>();
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
                    case nameof(CsvLogger):
                        return serviceProvider.GetService<CsvLogger>();
                    default:
                        return null;
                }
            });

            services.AddSingleton<ITradingEngine, TradingEngine>();
            serviceProvider = services.BuildServiceProvider();

            ITradingEngine engine = serviceProvider.GetService<ITradingEngine>();

            // read config file
            string configString = File.ReadAllText(args[0]);
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
    }
}
