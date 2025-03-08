using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Models.Results;

public class PortfolioSnapshot
{
    DateTime Timestamp;

    // 'real' values
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

    // 'calculated' values
    public double? Return; // return % from previous timestamp
    public double? ExcessReturn;
    public double? CumulativeReturn;
    public double? HighWaterMark;
    public double? CurrentDrawdown;
    public double? CurrentDrawdownDuration;
}
