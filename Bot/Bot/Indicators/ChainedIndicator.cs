// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;

namespace Bot.Indicators;

public class ChainedIndicator<T1_in, T1_out, T2_in, T2_out>(
    IIndicator<T1_in, T1_out> first,
    IIndicator<T2_in, T2_out> second,
    Func<T1_out, T2_in> selector) : IIndicator<T1_in, T2_out>
{
    private readonly IIndicator<T1_in, T1_out> _first = first;
    private readonly IIndicator<T2_in, T2_out> _second = second;
    private readonly Func<T1_out, T2_in> _selector = selector;

    public T2_out Value => _second.Value;
    public bool IsHydrated => _second.IsHydrated;

    public void Next(T1_in input)
    {
        _first.Next(input);

        if (_first.IsHydrated)
        {
            var mid = _selector(_first.Value);
            _second.Next(mid);
        }
    }

    public IIndicator<T_new_in, T2_out> Of<T_new_in, T_new_out>(
        IIndicator<T_new_in, T_new_out> source,
        Func<T_new_out, T1_in> selector)
    {
        return new ChainedIndicator<T_new_in, T_new_out, T1_in, T2_out>(source, this, selector);
    }

    public IIndicator<T_new_in, T2_out> Of<T_new_in>(IIndicator<T_new_in, T1_in> source)
    {
        return new ChainedIndicator<T_new_in, T1_in, T1_in, T2_out>(source, this, s => s);
    }

    public IIndicator<T1_in, T2_out> Of(Func<T1_in, T1_in> source)
    {
        return new ChainedIndicator<T1_in, T1_in, T1_in, T2_out>(new FuncIndicator<T1_in, T1_in>(source), this, s => s);
    }

    public IIndicator<T1_in, T_new_out> Into<T_new_in, T_new_out>(IIndicator<T_new_in, T_new_out> target, Func<T2_out, T_new_in> selector)
    {
        return new ChainedIndicator<T1_in, T2_out, T_new_in, T_new_out>(this, target, selector);
    }

    public IIndicator<T1_in, T_new_out> Into<T_new_out>(IIndicator<T2_out, T_new_out> target)
    {
        return new ChainedIndicator<T1_in, T2_out, T2_out, T_new_out>(this, target, s => s);
    }

}
