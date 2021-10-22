
using Bot.Analyzers.Loggers;

namespace Bot.Engine.Events
{
    public interface ILogReceiver
    {
        /// <summary>
        /// Receives log events.
        /// </summary>
        /// <param name="log"></param>
        void OnLog(string caller, string message, LogLevel level = LogLevel.Information);
    }
}
