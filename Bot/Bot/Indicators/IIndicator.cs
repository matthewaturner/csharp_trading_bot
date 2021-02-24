
using Bot.Brokers;

namespace Bot.Indicators
{
    public interface IIndicator
    {
        /// <summary>
        /// Gets the number of windows until the indicator is hydrated.
        /// </summary>
        public int Lookback { get; }

        /// <summary>
        /// Returns whether this indicator has accepted enough data to 
        /// be 'hydrated'.
        /// </summary>
        /// <returns></returns>
        public bool Hydrated { get; }

        /// <summary>
        /// Gets the value of the indicator at this point in time;
        /// </summary>
        public object Value { get; }

        /// <summary>
        /// Updates the indicator with a new row of data.
        /// </summary>
        /// <param name="tick"></param>
        public void OnTick(ITicks ticks);
    }
}
