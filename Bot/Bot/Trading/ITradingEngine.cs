using Bot.Brokerages;
using System;

namespace Bot.Trading.Interfaces
{
    public interface ITradingEngine
    {
        public void Run(string ticker, DateTime startDate, DateTime endDate, TickInterval tickInterval);

    }
}
