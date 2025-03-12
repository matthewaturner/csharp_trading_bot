// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

namespace Bot.Models.Broker;

public interface IAssetInformation
{
    /// <summary>
    /// Unique asset id according to the broker.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Class the asset belongs to.
    /// </summary>
    public string Class { get; set; }

    /// <summary>
    /// Exchange the asset is traded on.
    /// </summary>
    public string Exchange { get; set; }

    /// <summary>
    /// Symbol for the asset.
    /// </summary>
    public string Symbol { get; set; }

    /// <summary>
    /// Status of the asset. Active or inactive.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Whether the asset can be traded.
    /// </summary>
    public bool Tradable { get; set; }

    /// <summary>
    /// Whether the asset can be traded on margin.
    /// </summary>
    public bool Marginable { get; set; }

    /// <summary>
    /// Whether it is possible to short the asset.
    /// </summary>
    public bool Shortable { get; set; }

    /// <summary>
    /// Whether the asset is easy to borrow.
    /// </summary>
    public bool EasyToBorrow { get; set; }

    /// <summary>
    /// Whether this asset can be traded in fractional amounts.
    /// </summary>
    public bool Fractionable { get; set; }
}
