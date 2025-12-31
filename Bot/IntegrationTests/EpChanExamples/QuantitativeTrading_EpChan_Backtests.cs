
using Bot;
using Bot.Brokers.Backtest;
using Bot.DataSources.Csv;
using Bot.Models.Broker;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies.EpChan;
using Microsoft.Extensions.Logging;
using static Bot.Engine.TradingEngine;

namespace IntegrationTests.EpChanExamples;

/// <summary>
/// Reimplemented backtests for Quantitative Trading by Ernest Chan.
/// </summary>
public class QuantitativeTrading_EpChan_Backtests
{
    // number of decimal places to compare the results to
    const int ACCURACY = 12;

    // Example 3.4:
    // Buy and hold a share of IGE from 2001-11-26 to 2007-11-14
    // Calculate the sharpe ratio.
    [Fact]
    public async Task Example3_4()
    {
        var broker = new BacktestBroker(100000, ExecutionMode.OnCurrentBarClose);
        var builder = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new() { "IGE" },
                logLevel: LogLevel.Debug,
                annualRiskFreeRate: .04))
            .WithDataSource(new CsvDataSource(GlobalConfig.EpChanDataFolder))
            .WithStrategy(new Ex3_4_BuyAndHold("IGE"), 1.0)
            .WithExecutionEngine(broker);

        var engine = builder.Build();
        RunResult result = await engine.RunAsync();
        Assert.Equal(0.789317538344851, result.AnnualizedSharpeRatio, ACCURACY);
    }

    // Example 3.5:
    // Buy one share of IGE and short an equal dollar amount of SPY over the same period.
    // Calculate the sharpe ratio, maximum drawdown, and maximum drawdown duration.
    [Fact]
    public async Task Example3_5()
    {
        var broker = new BacktestBroker(100000, ExecutionMode.OnCurrentBarClose);
        var builder = new EngineBuilder()
         .WithConfig(new RunConfig(
             interval: Interval.OneDay,
             runMode: RunMode.BackTest,
             universe: new() { "IGE", "SPY" },
             logLevel: LogLevel.Debug))
         .WithDataSource(new CsvDataSource(GlobalConfig.EpChanDataFolder))
         .WithStrategy(new Ex3_5_LongShort("IGE", "SPY"), 1.0)
         .WithExecutionEngine(broker);

        var engine = builder.Build();
        RunResult result = await engine.RunAsync();
        Assert.Equal(0.78368110018119, result.AnnualizedSharpeRatio, ACCURACY);
        Assert.Equal(-0.0952926804720874, result.MaximumDrawdown, ACCURACY);
        Assert.Equal(497, result.MaximumDrawdownDuration);
    }

}