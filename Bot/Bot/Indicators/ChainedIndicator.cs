
using System;

namespace Bot.Indicators;

/// <summary>
/// Indicator that combines two indicators into a single indicator. Can be used repeatedly to form
/// long chains of custom indicators and really any form of data manipulation that is needed.
/// </summary>
public class ChainedIndicator<T1_in, T1_out, T2_in, T2_out>(
    IIndicator<T1_in, T1_out> first,
    IIndicator<T2_in, T2_out> second,
    Func<T1_out, T2_in> selector) : IIndicator<T1_in, T2_out>
{
    private readonly IIndicator<T1_in, T1_out> _first = first;
    private readonly IIndicator<T2_in, T2_out> _second = second;
    private readonly Func<T1_out, T2_in> _selector = selector;
    private readonly int _totalLookback = first.Lookback + second.Lookback;

    public int Lookback => _totalLookback;

    public bool IsHydrated => _second.IsHydrated;

    public T2_out Value => _second.Value;

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
}
