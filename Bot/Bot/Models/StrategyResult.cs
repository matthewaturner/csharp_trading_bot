using System;
using System.Collections.Generic;

namespace Bot.Models;

class StrategyResult
{
    public List<(DateTime, double)> PortfolioValue { get; set; }

    /// <summary>
    /// Adds the value for a particular datetime.
    /// </summary>
    public void AddPortfolioValue(DateTime dateTime, double value)
    {
        PortfolioValue.Add((dateTime, value));
    }
}
