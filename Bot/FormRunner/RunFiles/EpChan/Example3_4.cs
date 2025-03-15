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

namespace FormRunner.RunFiles.EpChan;

public class Example3_4
{
    public Form Run()
    {
        var builder = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new() { "IGE" },
                minLogLevel: LogLevel.Debug,
                shouldWriteCsv: true,
                annualRiskFreeRate: .04))
            .WithDataSource(new CsvDataSource(GlobalConfig.EpChanDataFolder))
            .WithStrategy(new Ex3_4_BuyAndHold("IGE"), 1.0);

        var engine = builder.Build();
        RunResult result = engine.RunAsync().Result;
        return new BacktestResultForm(result);
    }
}
