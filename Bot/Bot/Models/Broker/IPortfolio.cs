using Bot.Models.Results;
using System;

namespace Bot.Models.Broker;

public interface IPortfolio
{
    public string AccountId { get; }

    public double InitialCapital { get; }

    public double Cash { get; }

    public double PortfolioValue { get; }

    public double LongPositionsValue { get; }

    public double ShortPositionsValue { get; }

    public double GrossExposure { get; }

    public double NetExposure { get; }

    public double CapitalAtRisk { get; }

    public double Leverage { get; }

    public double RealizedPnL { get; }

    public double UnrealizedPnL { get; }

    public PortfolioSnapshot GetSnapshot(DateTime timestamp);
}
