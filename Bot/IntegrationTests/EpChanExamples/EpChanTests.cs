// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot;
using Bot.DataSources.Csv;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies.EpChan;
using Microsoft.Extensions.Logging;
using static Bot.Engine.TradingEngine;

namespace IntegrationTests.EpChanExamples;

public class EpChanTests
{
    // number of decimal places to compare the results to
    const int ACCURACY = 12;

    [Fact]
    public async Task Example3_4()
    {
        var builder = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new() { "IGE" },
                minLogLevel: LogLevel.Debug,
                annualRiskFreeRate: .04))
            .WithDataSource(new CsvDataSource(GlobalConfig.EpChanDataFolder))
            .WithStrategy(new Ex3_4_BuyAndHold("IGE"), 1.0);

        var engine = builder.Build();
        RunResult result = await engine.RunAsync();
        Assert.Equal(0.789317538344851, result.AnnualizedSharpeRatio, ACCURACY);
    }

    [Fact]
    public async Task Example3_5()
    {
        var builder = new EngineBuilder()
         .WithConfig(new RunConfig(
             interval: Interval.OneDay,
             runMode: RunMode.BackTest,
             universe: new() { "IGE", "SPY" },
             minLogLevel: LogLevel.Debug,
             shouldWriteCsv: true))
         .WithDataSource(new CsvDataSource(GlobalConfig.EpChanDataFolder))
         .WithStrategy(new Ex3_5_LongShort("IGE", "SPY"), 1.0);

        var engine = builder.Build();
        RunResult result = await engine.RunAsync();
        Assert.Equal(0.78368110018119, result.AnnualizedSharpeRatio, ACCURACY);
    }

}