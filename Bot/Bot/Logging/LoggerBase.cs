using System;

namespace Bot.Logging
{
    public abstract class LoggerBase
    {
        LogLevel configured;

        public LoggerBase(LogLevel logLevel = LogLevel.Information)
        { 
            configured = logLevel;
        }

        /// <summary>
        /// Implement this in the child class.
        /// </summary>
        public abstract void Log(object log, LogLevel msgLevel);

        public void LogVerbose(object msg)
        {
            this.Log(msg, LogLevel.Verbose);
        }

        public void LogInformation(object msg)
        {
            this.Log(msg, LogLevel.Information);
        }

        public void LogWarning(object msg)
        {
            this.Log(msg, LogLevel.Warning);
        }

        public void LogError(object msg)
        {
            this.Log(msg, LogLevel.Error);
        }
    }
}