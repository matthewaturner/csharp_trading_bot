// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System;

namespace Bot.Models.Results;

public class PortfolioSnapshot
{
    public DateTime Timestamp;
    public double Cash;
    public double PortfolioValue;
    public double LongPositionsValue;
    public double ShortPositionsValue;
    public double GrossExposure;
    public double NetExposure;
    public double CapitalAtRisk;
    public double Leverage;
    public double RealizedPnL;
    public double UnrealizedPnL;
}
