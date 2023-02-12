
namespace Bot.Logging
{
    public interface ILogger
    {
        void LogVerbose(string msg);

        void LogInformation(string msg);

        void LogWarning(string msg);

        void LogError(string msg);
    }
}