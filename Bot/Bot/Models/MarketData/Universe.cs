﻿using System;

namespace Bot.Models.MarketData;

/// <summary>
/// Class representing all available symbols for the run.
/// </summary>
public class Universe
{
    public Universe(params string[] symbols)
    {
        AllSymbols = symbols;
    }

    public string[] AllSymbols { get; }
}
