using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.Allocations;

public class MetaAllocation
{
    private readonly Dictionary<string, (Allocation allocation, double weight)> _metaAllocations = new();

    // Add or update an allocation with its meta weight and strategy ID
    public void SetAllocation(string strategyId, Allocation allocation, double weight)
    {
        if (string.IsNullOrWhiteSpace(strategyId))
            throw new ArgumentException("Strategy ID cannot be null or empty");

        _metaAllocations[strategyId] = (allocation, weight);
    }

    // Get meta weight of a specific strategy ID
    public double GetAllocationWeight(string strategyId)
    {
        return _metaAllocations.TryGetValue(strategyId, out var entry) ? entry.weight : 0;
    }

    // Get allocation by strategy ID
    public Allocation GetAllocation(string strategyId)
    {
        return _metaAllocations.TryGetValue(strategyId, out var entry) ? entry.allocation : null;
    }

    // Return all meta allocations
    public IReadOnlyDictionary<string, (Allocation allocation, double weight)> GetAllMetaAllocations() => _metaAllocations;

    // Total weight of all meta allocations (should be 1.0 if valid)
    public double TotalWeight() => _metaAllocations.Values.Sum(entry => entry.weight);

    // Merge all allocations into a flat symbol -> total weight map
    public Dictionary<string, double> FlattenAllocations()
    {
        var combined = new Dictionary<string, double>();

        foreach (var (strategyId, (alloc, metaWeight)) in _metaAllocations)
        {
            foreach (var (symbol, weight) in alloc.GetAllAllocations())
            {
                if (!combined.ContainsKey(symbol))
                    combined[symbol] = 0;

                combined[symbol] += weight * metaWeight;
            }
        }

        return combined;
    }
} 
