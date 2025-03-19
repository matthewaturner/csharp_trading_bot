// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

namespace Bot.Indicators;

public interface IIndicator<T_in, T_out>
{
    public T_out Value { get; }

    public bool IsHydrated { get; }
}