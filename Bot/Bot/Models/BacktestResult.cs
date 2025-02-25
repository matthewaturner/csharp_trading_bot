using System;
using System.Collections.Generic;

namespace Bot.Models;

class BacktestResult
{
    public List<(DateTime, double)> PortfolioValue { get; set; }
}
