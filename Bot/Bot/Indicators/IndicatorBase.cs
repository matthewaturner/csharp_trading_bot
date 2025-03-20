using Bot.Exceptions;
using System;

namespace Bot.Indicators;

public abstract class IndicatorBase<T_in, T_out>(int lookback = 1) : IIndicator<T_in, T_out>
{
    private readonly int _lookback = lookback;
    private int _count = 0;
    protected T_out _value;

    /// <summary>
    /// Safely get the actual value of the indicator.
    /// </summary>
    public T_out Value => IsHydrated ? _value : throw new NotHydratedException();
    
    /// <summary>
    /// Whether the indicator is hydrated or not.
    /// </summary>
    public bool IsHydrated { get; protected set; }

    /// <summary>
    /// Base add method.
    /// </summary>
    public  void Add(T_in input)
    {
        _count++;
        OnAdd(input);
        IsHydrated = _count >= _lookback;
    }

    /// <summary>
    /// Compose from another indicator, with a transformation in between.
    /// </summary>
    public IIndicator<T_new_in, T_out> Of<T_new_in, T_new_out>(IIndicator<T_new_in, T_new_out> source, Func<T_new_out, T_in> selector)
    {
        return new ChainedIndicator<T_new_in, T_new_out, T_in, T_out>(source, this, selector);
    }

    /// <summary>
    /// Compose directly from another indicator.
    /// </summary>
    public IIndicator<T_new_in, T_out> Of<T_new_in>(IIndicator<T_new_in, T_in> source)
    {
        return new ChainedIndicator<T_new_in, T_in, T_in, T_out>(source, this, s => s);
    }

    /// <summary>
    /// Compose from a func.
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public IIndicator<T_in, T_out> Of(Func<T_in, T_in> source)
    {
        return new ChainedIndicator<T_in, T_in, T_in, T_out>(new FuncIndicator<T_in, T_in>(source), this, s => s);
    }

    /// <summary>
    /// Abstract on add to be implemented by the child class.
    /// </summary>
    public abstract void OnAdd(T_in input);
}
