
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.Allocations;

public class Allocation
{
    private Dictionary<string, double> _allocations = new();
    
    public void SetAllocation(string symbol, double weight)
    {
        _allocations[symbol] = weight;
    }
    
    public double GetAllocation(string symbol)
    {
        return _allocations.TryGetValue(symbol, out var weight) ? weight : 0;
    }
    
    public IReadOnlyDictionary<string, double> GetAllAllocations() => _allocations;
    
    public double TotalWeight() => _allocations.Values.Sum();
}