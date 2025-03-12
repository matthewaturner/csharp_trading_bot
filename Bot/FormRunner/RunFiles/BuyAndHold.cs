// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Analyzers;
using Bot.DataSources.Alpaca;
using Bot.Engine;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using Bot.Models.Results;
using Bot.Strategies.EpChan;
using static Bot.Engine.TradingEngine;

namespace FormRunner.RunFiles;

public class BuyAndHold
{
    public Form Run()
    {
        var builder = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                start: new DateTime(2000, 1, 1),
                end: DateTime.Now,
                universe: new() { "XOM" }))
            .WithDataSource(new AlpacaDataSource())
            .WithStrategy(new Ex3_4_BuyAndHold("XOM"), 1.0);

        var engine = builder.Build();

        RunResult result = engine.RunAsync().Result;

        return new ScatterPlotForm(result);
    }
}
