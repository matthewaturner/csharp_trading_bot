
using Bot.DataStorage.Models;

namespace Bot.Indicators
{
    public interface IIndicator<T>
    {
        /// <summary>
        /// Updates the indicator with a new row of data.
        /// </summary>
        /// <param name="tick"></param>
        public void OnTick(Tick tick);

        /// <summary>
        /// Gets the value of the indicator at the current time step.
        /// </summary>
        public T Value { get; }
    }
}
