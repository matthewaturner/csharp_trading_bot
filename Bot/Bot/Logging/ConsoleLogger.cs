namespace Bot.Logging
{
    public class ConsoleLogger : LoggerBase, ILogger
    {
        LogLevel configured;

        public ConsoleLogger(LogLevel logLevel = LogLevel.Information)
        { 
            configured = logLevel;
        }

        /// <summary>
        /// Log to the console.
        /// </summary>
        public override void Log(object msg, LogLevel msgLevel)
        {
            if (msgLevel >= configured)
            {
                System.Console.WriteLine($"{configured.ToString().ToUpper()}:: {msg}");
            }
        }

    }
}