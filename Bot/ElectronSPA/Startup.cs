using Bot.Analyzers;
using Bot.Configuration;
using Bot.Data;
using Bot.Engine;
using Bot.Models;
using Bot.Strategies;
using Core;
using Core.Configuration;
using ElectronNET.API;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Data.Common;
using System.Net.Http;
using static Bot.Program;

namespace ElectronSPA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

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
            services.AddSingleton<IDataProcessor, BackTestDataProcessor>();
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            Bootstrap();
        }

        public async void Bootstrap()
        {
            await Electron.WindowManager.CreateWindowAsync();
        }
    }
}
