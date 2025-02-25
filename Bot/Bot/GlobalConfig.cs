using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace Bot;

/// <summary>
/// Gives a static way to access global configuration.
/// Global config stores things like api keys, etc. that are not specific to a strategy.
/// </summary>
public static class GlobalConfig
{
    // Path to the data folder
    public static string EpChanDataFolder = Path.Combine(
        AppContext.BaseDirectory,
        "..", "..", "..", "..", "..", "Data", "epchan");

    private static IConfiguration Config { get; set; }

    static GlobalConfig()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        Config = builder.Build();
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
