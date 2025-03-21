// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.Models.Allocations;

public class Allocation : Dictionary<string, double>
{
    // Constructors

    public Allocation()
    { }

    public Allocation(Dictionary<string, double> dictionary) 
        : base(dictionary) { }

    public static Allocation Empty(params string[] symbols)
    {
        return new Allocation(symbols.ToDictionary(s => s, s => 0.0));
    }

    // Methods

    public void SetAllocation(string symbol, double weight)
    {
        this[symbol] = weight;
    }

    public double GetAllocation(string symbol)
    {
        return this.TryGetValue(symbol, out var weight) ? weight : 0;
    }

    public double TotalWeight() => this.Values.Sum();

    public double AbsoluteWeight() => this.Values.Select(s => Math.Abs(s)).Sum();

    public Allocation ToUnitPortfolio()
    {
        foreach (string symbol in this.Keys)
        {
            SetAllocation(symbol, this[symbol] / AbsoluteWeight());
        }
        return this;
    }

    // Math Operations

    public static Allocation operator +(Allocation a, Allocation b)
    {
        var result = new Allocation();

        // Add weights from the first allocation
        foreach (var kvp in a)
        {
            result[kvp.Key] = kvp.Value;
        }

        // Add weights from the second allocation
        foreach (var kvp in b)
        {
            if (result.ContainsKey(kvp.Key))
            {
                result[kvp.Key] += kvp.Value;
            }
            else
            {
                result[kvp.Key] = kvp.Value;
            }
        }

        return result;
    }

    public static Allocation operator -(Allocation a, Allocation b)
    {
        var result = new Allocation();

        // Add weights from the first allocation
        foreach (var kvp in a)
        {
            result[kvp.Key] = kvp.Value;
        }

        // Subtract weights from the second allocation
        foreach (var kvp in b)
        {
            if (result.ContainsKey(kvp.Key))
            {
                result[kvp.Key] -= kvp.Value;
            }
            else
            {
                result[kvp.Key] = -kvp.Value;
            }
        }

        return result;
    }

    public static Allocation operator -(Allocation a)
    {
        var result = new Allocation();

        // Negate weights
        foreach (var kvp in a)
        {
            result[kvp.Key] = -kvp.Value;
        }

        return result;
    }
}