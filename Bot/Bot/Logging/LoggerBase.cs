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
        public abstract void Log(string msg, LogLevel msgLevel);

        public void LogVerbose(string msg)
        {
            this.Log(msg, LogLevel.Verbose);
        }

        public void LogInformation(string msg)
        {
            this.Log(msg, LogLevel.Information);
        }

        public void LogWarning(string msg)
        {
            this.Log(msg, LogLevel.Warning);
        }

        public void LogError(string msg)
        {
            this.Log(msg, LogLevel.Error);
        }
    }
}