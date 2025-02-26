using Bot.Models;

namespace Bot.Engine.Events
{
    public interface IBarReceiver
    {
        /// <summary>
        /// Function to execute when new bars come in.
        /// </summary>
        public void BaseOnBar(MultiBar bars);
    }
}
