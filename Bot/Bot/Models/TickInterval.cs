using System;

namespace Bot.Brokerages
{
    public enum TickInterval
    {
        Day = 0,
        Hour = 1,
        Minute = 2
    }

    public static class TickIntervalMapping
    {
        public static TimeSpan MapToTimeSpan(TickInterval tickInterval)
        {
            switch (tickInterval)
            {
                case TickInterval.Day:
                    return new TimeSpan(1, 0, 0, 0);
                case TickInterval.Hour:
                    return new TimeSpan(1, 0, 0);
                case TickInterval.Minute:
                    return new TimeSpan(0, 1, 0);
                default:
                    throw new NotImplementedException("Error: TickIntervalMapping.MapToTimeSpan not implement for " + tickInterval);
            }
        }
    }
}
