
using System;

namespace Bot.Indicators;

public interface IIndicator<T_in, T_out>
{
    int Lookback { get; }

    bool IsHydrated { get; }

    T_out Value { get; }

    void Next(T_in input);

    /// <summary>
    /// Compose from another indicator. prev -> this
    /// </summary>
    public IIndicator<T_new_in, T_out> Of<T_new_in, T_new_out>(
        IIndicator<T_new_in, T_new_out> source,
        Func<T_new_out, T_in> selector);

    /// <summary>
    /// Compose from another indicator.
    /// </summary>
    public IIndicator<T_new_in, T_out> Of<T_new_in>(
        IIndicator<T_new_in, T_in> source);

    /// <summary>
    /// Compose from a func indicator.
    /// </summary>
    public IIndicator<T_in, T_out> Of(Func<T_in, T_in> source);
}
