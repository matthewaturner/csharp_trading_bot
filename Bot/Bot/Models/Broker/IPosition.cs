// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

namespace Bot.Models.Broker;

public interface IPosition
{
    /// <summary>
    /// Symbol.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// Amount held.
    /// </summary>
    public double Quantity { get; }
}
