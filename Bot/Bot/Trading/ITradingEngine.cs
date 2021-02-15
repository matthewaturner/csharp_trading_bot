using Bot.Models;
using System;

namespace Bot.Trading.Interfaces
{
    public interface ITradingEngine
    {
        public void Run(DateTime startDate, DateTime endDate, TickInterval tickInterval);

    }
}
