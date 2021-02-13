using Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace Bot
{
    class Program
    {
        /// <summary>
        /// Initializes an instance of the program.
        /// </summary>
        /// <param name="configuration"></param>
        public Program(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        public void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient();

            IServiceCollection services = new ServiceCollection();

            // inject singletons
            services.AddLogging();
            services.AddSingleton<HttpClient>();

            // setup configurations
            services.Configure<YahooDataConfiguration>(Configuration.GetSection(ConfigurationPaths.Yahoo));

            IServiceProvider provider = services.BuildServiceProvider();
        }


    }
}
