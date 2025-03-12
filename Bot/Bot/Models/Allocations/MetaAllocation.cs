// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.Allocations;

public class MetaAllocation
{
    private readonly Dictionary<string, double> _strategyWeights = new();
    private readonly Dictionary<string, Allocation> _strategyAllocations = new();

    /// <summary>
    /// Set the weight for a given strategy.
    /// </summary>
    public void SetStrategyWeight(string strategyId, double weight)
    {
        if (string.IsNullOrWhiteSpace(strategyId))
            throw new ArgumentException("Strategy ID cannot be null or empty");
        if (weight < 0 || weight > 1)
            throw new ArgumentOutOfRangeException(nameof(weight), "Strategy weight must be between 0 and 1");

        _strategyWeights[strategyId] = weight;
    }

    /// <summary>
    /// Get the weight of a particular strategy.
    /// </summary>
    /// <param name="strategyId"></param>
    /// <returns></returns>
    public double GetStrategyWeight(string strategyId)
    {
        return _strategyWeights.TryGetValue(strategyId, out double weight) ?
            weight : throw new KeyNotFoundException($"Strategy id {strategyId} not found.");
    }

    /// <summary>
    /// Set the allocation for a particular strategy.
    /// </summary>
    public void SetStrategyAllocation(string strategyId, Allocation allocation)
    {
        if (string.IsNullOrWhiteSpace(strategyId))
            throw new ArgumentException("Strategy ID cannot be null or empty");
        if (allocation == null)
            throw new ArgumentNullException("Allocation was null.");

        _strategyAllocations[strategyId] = allocation;
    }

    /// <summary>
    /// Get the allocation for a particular strategy.
    /// </summary>
    public Allocation GetAllocation(string strategyId)
    {
        return _strategyAllocations.TryGetValue(strategyId, out var allocation) ?
            allocation : throw new KeyNotFoundException($"Strategy id {strategyId} not found.");
    }

    /// <summary>
    /// Sum up the weight of all strategies.
    /// </summary>
    public double TotalWeight() => _strategyWeights.Values.Sum();

    /// <summary>
    /// Merge all the different allocations into one large allocation dictionary
    /// </summary>
    /// <returns></returns>
    public Allocation FlattenAllocations()
    {
        var combined = new Dictionary<string, double>();

        foreach ((string strategyId, double strategyWeight) in _strategyWeights)
        {
            foreach ((string symbol, double weight) in _strategyAllocations[strategyId])
            {
                if (!combined.ContainsKey(symbol))
                    combined[symbol] = 0;

                combined[symbol] += weight * strategyWeight;
            }
        }

        return combined;
    }
}
