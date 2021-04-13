
using Bot.Models;
using System.Collections.Generic;

namespace Bot.Indicators
{
    public interface IIndicator
    {
        /// <summary>
        /// Gets the number of windows until the indicator is hydrated.
        /// </summary>
        int Lookback { get; }

        /// <summary>
        /// Returns whether this indicator has accepted enough data to 
        /// be 'hydrated'.
        /// </summary>
        /// <returns></returns>
        bool Hydrated { get; }

        /// <summary>
        /// Updates the indicator with a new row of data.
        /// </summary>
        /// <param name="tick"></param>
        void OnTick(IMultiTick ticks);

        /// <summary>
        /// Gets the default value of the indicator.
        /// Equivalent to this["default"]
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Accessor for named values the indicator may output.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        double this[string valueName] { get; }
    }
}
