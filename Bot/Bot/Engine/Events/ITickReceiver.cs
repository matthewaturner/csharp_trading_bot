using Bot.Models;

namespace Bot.Engine.Events
{
    public interface ITickReceiver
    {
        /// <summary>
        /// Function to execute when new ticks come in.
        /// </summary>
        public void BaseOnTick(IMultiBar ticks);
    }
}
