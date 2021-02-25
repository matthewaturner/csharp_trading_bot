using Bot.Configuration;
using Bot.DataCollection;
using Bot.DataStorage;
using Bot.DataStorage.Models;
using Bot.Models;
using Core;
using Core.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using RDotNet;
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
            services.Configure<TickContext>(configuration.GetSection(ConfigurationPaths.Sql));
            services.Configure<KeyVaultConfiguration>(configuration.GetSection(ConfigurationPaths.KeyVault));

            // entity framework stuff
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);

            // inject singletons
            services.AddSingleton<IKeyVaultManager, KeyVaultManager>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<IDataSource, YahooDataSource>();
            services.AddSingleton<ITickStorage, TickStorage>();

            IServiceProvider provider = services.BuildServiceProvider();

            TestStuff(provider);
        }

        public static void TestStuff(IServiceProvider provider)
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
