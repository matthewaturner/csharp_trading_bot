using Bot.Configuration;
using Bot.Models;
using Bot.Strategies;
using Bot.Engine;
using Core;
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
            string outputPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/CSharpTradeBot";

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
            services.AddSingleton<HttpClient>();

            // inject data sources
            services.AddSingleton<YahooDataSource>();

            // inject brokers
            services.AddSingleton<BackTestingBroker>();

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

            var engineConfig = new EngineConfig()
            {
                Symbols = new List<string>() { "FLJH", "EWJE", "FLTW" },
                Interval = TickInterval.Day,
                Start = new DateTime(2020, 1, 1),
                End = new DateTime(2021, 3, 21),
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
                    Name = "BollingerMeanReversion",
                    Args = new string[] { "10", "1", ".05", "2.90852161;-2.22481014;-.00132392013" }
                },
                Analyzers = new List<DependencyConfig>()
                {
                    new DependencyConfig()
                    {
                        Name = "CsvLogger",
                        Args = new string[] { outputPath }
                    },
                    new DependencyConfig()
                    {
                        Name = "SharpeRatio",
                        Args = new string[] { "0.00005357"}
                    },
                }
            };

            /*
            engine.Initialize(engineConfig);
            engine.RunAsync().Wait();
            */
            TestStuff();
        }

        public static void TestStuff()
        {
            try
            {
                StartupParameter rinit = new StartupParameter();
                rinit.Quiet = true;
                rinit.RHome = "C:/Program Files/R/R-3.4.3";
                rinit.Interactive = true;
                REngine rEngine = REngine.GetInstance(null, true, rinit);

                // A somewhat contrived but customary Hello World:
                CharacterVector charVec = rEngine.CreateCharacterVector(new[] { "Hello, R world!, .NET speaking" });
                rEngine.SetSymbol("greetings", charVec);
                rEngine.Evaluate("str(greetings)"); // print out in the console
                string[] a = rEngine.Evaluate("'Hi there .NET, from the R engine'").AsCharacter().ToArray();
                Console.WriteLine("R answered: '{0}'", a[0]);
                Console.WriteLine("Press any key to exit the program");
                Console.ReadKey();
                rEngine.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
