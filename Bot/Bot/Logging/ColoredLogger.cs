using Microsoft.Extensions.Logging;
using System;

namespace Bot.Logging;

/// <summary>
/// Colored logger adds special coloring to different log levels (debug, info, warning, error).
/// </summary>
/// <param name="innerLogger"></param>
public class ColoredLogger(ILogger innerLogger) : ILogger
{
    private readonly ILogger _innerLogger = innerLogger;

    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return _innerLogger.BeginScope(state);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return _innerLogger.IsEnabled(logLevel);
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        string message = formatter(state, exception);
        string coloredMessage = $"{GetAnsiColorForLogLevel(logLevel)}{message}{AnsiReset}";

        _innerLogger.Log(logLevel, eventId, coloredMessage, exception, (s, e) => s);
    }

    private static string GetAnsiColorForLogLevel(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Debug => "\u001b[90m",       // Gray
            LogLevel.Information => "\u001b[97m", // White
            LogLevel.Warning => "\u001b[93m",     // Yellow
            LogLevel.Error => "\u001b[91m",       // Red
            LogLevel.Critical => "\u001b[31m",    // Dark Red
            _ => "\u001b[97m" // Default White
        };
    }

    private const string AnsiReset = "\u001b[0m";
}


