using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Bot;

/// <summary>
/// Gives a static way to access global configuration.
/// Global config stores things like api keys, etc. that are not specific to a strategy.
/// </summary>
public static class GlobalConfig
{
    private static IConfiguration Config { get; set; }

    public static ILogger Logger { get; private set; }

    public static string EpChanDataFolder = Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "..", "..", "Data", "epchan");

    /// <summary>
    /// Constructor.
    /// </summary>
    static GlobalConfig()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        Config = builder.Build();

        // get the loglevel from appsettings
        Enum.TryParse(GetValue("LogLevel"), out LogLevel logLevel);

        Logger = LoggerFactory.Create(builder =>
        {
            builder
                .AddSimpleConsole(options =>
                {
                    options.IncludeScopes = true;
                    options.TimestampFormat = "[HH:mm:ss] ";
                })
                .AddConsole()
                .SetMinimumLevel(logLevel);
        }).CreateLogger("Global");
    }

    /// <summary>
    /// Get a single value from config.
    /// </summary>
    public static string GetValue(string key) => Config[key];

    /// <summary>
    /// Get a section of the config and cast it to a config object.
    /// </summary>
    public static T GetSection<T>(string key) where T : new()
    {
        var section = new T();
        Config.GetSection(key).Bind(section);
        return section;
    }
}
