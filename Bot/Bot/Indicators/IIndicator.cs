
using Bot.Brokerages;

namespace Bot.Indicators
{
    public interface IIndicator<T>
    {
        /// <summary>
        /// Gets the value of the indicator at the current time step.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// Updates the indicator with a new row of data.
        /// </summary>
        /// <param name="tick"></param>
        public void OnTick(Tick tick);

        /// <summary>
        /// Returns whether this indicator has accepted enough data to 
        /// be 'hydrated'.
        /// </summary>
        /// <returns></returns>
        public bool IsHydrated();
    }
}
