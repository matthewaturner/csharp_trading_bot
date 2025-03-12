// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Broker;

namespace Bot.Brokers.Backtest.Models;

public class BacktestPosition : IPosition
{
    /// <summary>
    /// Constructor.
    /// </summary>
    public BacktestPosition()
    { }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="quantity"></param>
    public BacktestPosition(
        string symbol,
        double quantity)
    {
        Symbol = symbol.ToUpper();
        Quantity = quantity;
        Type = quantity > 0 ? PositionType.Long : PositionType.Short;
    }

    /// <summary>
    /// Symbol referred to.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    /// Number of units. Can be fractional.
    /// </summary>
    public double Quantity { get; set; }

    /// <summary>
    /// Long or short.
    /// </summary>
    public PositionType Type { get; set; }

    /// <summary>
    /// ToString.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"(Symbol:{Symbol} Quantity:{Quantity})";
    }
}
