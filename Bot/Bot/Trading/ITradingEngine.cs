using System;
using System.Collections.Generic;
using System.Text;

namespace Bot.Trading.Interfaces
{
    public interface ITradingEngine
    {
        public void Run(DateTime startDate, DateTime endDate, TimeSpan timeSpan);

    }
}
