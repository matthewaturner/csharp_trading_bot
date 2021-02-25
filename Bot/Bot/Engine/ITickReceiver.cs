
using Bot.Models;

namespace Bot.Engine
{
    public interface ITickReceiver
    {
        /// <summary>
        /// Function to execute when new ticks come in.
        /// </summary>
        public void OnTick(ITicks ticks);
    }
}
