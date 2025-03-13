// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.Allocations;

public class Allocation : Dictionary<string, double>
{

    public void SetAllocation(string symbol, double weight)
    {
        this[symbol] = weight;
    }

    public double GetAllocation(string symbol)
    {
        return this.TryGetValue(symbol, out var weight) ? weight : 0;
    }

    public double TotalWeight() => this.Values.Sum();
}