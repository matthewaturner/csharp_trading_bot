using Theo.Models;

namespace Theo.Engine.Events
{
    public interface ITickReceiver
    {
        /// <summary>
        /// Function to execute when new ticks come in.
        /// </summary>
        public void BaseOnTick(IMultiTick ticks);
    }
}
