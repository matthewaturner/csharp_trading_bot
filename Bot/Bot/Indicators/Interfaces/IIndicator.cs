﻿
using Bot.Models;

namespace Bot.Indicators
{
    public interface IIndicator
    {
        /// <summary>
        /// Gets the name of the indicator.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the number of windows until the indicator is hydrated.
        /// </summary>
        int Lookback { get; }

        /// <summary>
        /// Returns whether this indicator has accepted enough data to 
        /// be 'hydrated'.
        /// </summary>
        /// <returns></returns>
        bool IsHydrated { get; }

        /// <summary>
        /// Updates the indicator with a new row of data.
        /// </summary>
        /// <param name="bar"></param>
        void OnBar(MultiBar bars);
    }
}
