// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot;
using Bot.DataSources.Csv;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using Bot.Models.Results;
using Bot.Strategies.EpChan;
using Microsoft.Extensions.Logging;
using static Bot.Engine.TradingEngine;

namespace IntegrationTests.EpChanExamples;

public class EpChanTests
{
    [Test]
    public void Example3_4()
    {
        var builder = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new() { "IGE" },
                minLogLevel: LogLevel.Debug,
                shouldWriteCsv: true))
            .WithDataSource(new CsvDataSource(GlobalConfig.EpChanDataFolder))
            .WithStrategy(new Ex3_4_BuyAndHold("IGE"), 1.0);

        var engine = builder.Build();
        RunResult result = engine.RunAsync().Result;
        Assert.AreEqual(0.789317538, result.AnnualizedSharpeRatio, 9);
    }

}