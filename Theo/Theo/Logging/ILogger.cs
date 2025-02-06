
namespace Theo.Logging
{
    public interface ILogger
    {
        void LogVerbose(object msg);

        void LogInformation(object msg);

        void LogWarning(object msg);

        void LogError(object msg);
    }
}