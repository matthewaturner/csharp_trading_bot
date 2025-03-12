// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

namespace Bot.Models.Allocations;

public struct Percentage
{
    private readonly double value;

    public Percentage(int percent)
    {
        value = percent / 100.0;
    }

    private Percentage(double value)
    {
        this.value = value;
    }

    public static implicit operator double(Percentage p) => p.value;
    public static implicit operator Percentage(double d) => new Percentage(d);
    
    public override string ToString() => $"{value:P}";
}
