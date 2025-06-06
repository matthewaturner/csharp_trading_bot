﻿// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

namespace Bot.Models.Broker;

public interface IOrderRequest
{
    /// <summary>
    /// Type of the order to place.
    /// </summary>
    public OrderType Type { get; }

    /// <summary>
    /// Symbol the order is for.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// Quantity to buy or sell.
    /// </summary>
    public double Quantity { get; }
}
