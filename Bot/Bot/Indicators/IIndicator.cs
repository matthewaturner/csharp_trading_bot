// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;

namespace Bot.Indicators;

public interface IIndicator<T_in, T_out>
{
    bool IsHydrated { get; }

    T_out Value { get; }

    void Add(T_in input);

    /// <summary>
    /// Compose from another indicator.
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
    /// Compose from another indicator.
    /// </summary>
    public IIndicator<T_in, T_out> Of(Func<T_in, T_in> source);
}
