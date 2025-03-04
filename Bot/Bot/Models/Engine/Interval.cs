using System;

namespace Bot.Models.Engine;

/// <summary>
/// Class representing an interval of time, usually for a bar.
/// </summary>
public class Interval
{
    private enum IntervalType { Month, Week, Day, Hour, Minute }

    private readonly IntervalType _type;
    private readonly int _num;

    private Interval(IntervalType type, int num)
    {
        if (num < 1) throw new ArgumentException("Interval value must be > 0");
        if (type == IntervalType.Month && num > 12) throw new ArgumentException("Interval value for months must be <= 12");
        if (type == IntervalType.Week && num > 1) throw new ArgumentException("Interval value for weeks must be <= 1");
        if (type == IntervalType.Day && num > 1) throw new ArgumentException("Interval value for days must be <= 1");
        if (type == IntervalType.Hour && num > 23) throw new ArgumentException("Interval value for days must be <= 24");
        if (type == IntervalType.Minute && num > 59) throw new ArgumentException("Interval value for days must be <= 59");

        _type = type;
        _num = num;
    }

    public static Interval OneYear => new(IntervalType.Month, 12);
    public static Interval Months(int num) => new(IntervalType.Month, num);
    public static Interval OneWeek => new(IntervalType.Week, 1);
    public static Interval OneDay => new(IntervalType.Day, 1);
    public static Interval Hours(int num) => new(IntervalType.Hour, num);
    public static Interval Mins(int num) => new(IntervalType.Minute, num);

    public double GetIntervalsPerYear() => _type switch
    {
        IntervalType.Month => 252.0 / 12 / _num,
        IntervalType.Week => 252.0 / 5 / _num,
        IntervalType.Day => 252.0 / _num,            // 252 days of trading per year
        IntervalType.Hour => 6.5 * 252 / _num,         // 6.5 hours of trading per day
        IntervalType.Minute => 60.0 * 24 * 252 / _num,
        _ => throw new ArgumentOutOfRangeException()
    };

    public override string ToString() => _type switch
    {
        IntervalType.Month => $"{_num}M",
        IntervalType.Week => $"{_num}W",
        IntervalType.Day => $"{_num}D",
        IntervalType.Hour => $"{_num}H",
        IntervalType.Minute => $"{_num}T",
        _ => throw new ArgumentOutOfRangeException()
    };
}
