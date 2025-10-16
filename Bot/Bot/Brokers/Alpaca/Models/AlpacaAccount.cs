
using Bot.Models.Broker;
using Bot.Models.Results;
using Newtonsoft.Json;
using System;

namespace Bot.Brokers.Alpaca.Models;

public class AlpacaAccount : IPortfolio
{
    // Alpaca fields

    [JsonProperty("account_number")]
    public string AccountId { get; set; }

    [JsonProperty("buying_power")]
    public string BuyingPower { get; set; }

    [JsonProperty("cash")]
    public string Cash { get; set; }

    [JsonProperty("equity")]
    public string Equity { get; set; }

    // junk to make it build
    public double InitialCapital => throw new System.NotImplementedException();

    public double PortfolioValue => throw new System.NotImplementedException();

    public double LongPositionsValue => throw new System.NotImplementedException();

    public double ShortPositionsValue => throw new System.NotImplementedException();

    public double GrossExposure => throw new System.NotImplementedException();

    public double NetExposure => throw new System.NotImplementedException();

    public double CapitalAtRisk => throw new System.NotImplementedException();

    public double Leverage => throw new System.NotImplementedException();

    public double RealizedPnL => throw new System.NotImplementedException();

    public double UnrealizedPnL => throw new System.NotImplementedException();

    double IPortfolio.Cash => throw new System.NotImplementedException();

    public PortfolioSnapshot GetSnapshot(DateTime timestamp)
    {
        throw new NotImplementedException();
    }
}
