using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Bot;

/// <summary>
/// Gives a static way to access global configuration.
/// Global config stores things like api keys, etc. that are not specific to a strategy.
/// </summary>
public static class GlobalConfig
{
    private static IConfiguration Config { get; set; }

    public static ILoggerFactory LogFactory { get; private set; }

    public static LogLevel MinimumLogLevel { get; set; } = LogLevel.Information;

    public static string EpChanDataFolder = Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "..", "..", "Data", "epchan");

    public static string OutputFolder = Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "..", "..", "Output");

    /// <summary>
    /// Constructor.
    /// </summary>
    static GlobalConfig()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        Config = builder.Build();

        LogFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddSimpleConsole(options =>
                {
                    options.TimestampFormat = "[HH:mm:ss] ";
                    options.SingleLine = true;
                })
                .AddConsole()
                .SetMinimumLevel(MinimumLogLevel);
        });
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
