using Bot.Models;
using System;

namespace Bot.Trading
{
    public interface ITradingEngine
    {
        public void Run(string ticker, DateTime startDate, DateTime endDate, TickInterval tickInterval);

    }
}
