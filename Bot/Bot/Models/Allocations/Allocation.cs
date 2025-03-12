// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.Allocations;

public class Allocation : IEnumerable<KeyValuePair<string, double>>
{
    private Dictionary<string, double> _allocations = new();

    public Allocation()
    { }

    public Allocation(IEnumerable<KeyValuePair<string, double>> allocations)
    {
        foreach (var kvp in allocations)
        {
            _allocations[kvp.Key] = kvp.Value;
        }
    }

    public void SetAllocation(string symbol, double weight)
    {
        _allocations[symbol] = weight;
    }

    public double GetAllocation(string symbol)
    {
        return _allocations.TryGetValue(symbol, out var weight) ? weight : 0;
    }

    public double TotalWeight() => _allocations.Values.Sum();

    // Implicit conversion from Dictionary<string, double> to Allocation
    public static implicit operator Allocation(Dictionary<string, double> allocations)
    {
        var allocation = new Allocation();
        foreach (var kvp in allocations)
        {
            allocation.SetAllocation(kvp.Key, kvp.Value);
        }
        return allocation;
    }

    // Implicit conversion from Allocation to Dictionary<string, double>
    public static implicit operator Dictionary<string, double>(Allocation allocation)
    {
        return allocation._allocations;
    }

    // Enumerator so we can pretend this is just a dictionary
    public IEnumerator<KeyValuePair<string, double>> GetEnumerator()
    {
        return _allocations.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    // Indexer to access allocations by symbol
    public double this[string symbol]
    {
        get => GetAllocation(symbol);
        set => SetAllocation(symbol, value);
    }
}