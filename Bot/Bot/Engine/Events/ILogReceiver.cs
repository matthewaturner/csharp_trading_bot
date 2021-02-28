
namespace Bot.Engine.Events
{
    public interface ILogReceiver
    {
        /// <summary>
        /// Receives log events.
        /// </summary>
        /// <param name="log"></param>
        void OnLog(string log);
    }
}
