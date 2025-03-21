// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.DataSources.Alpaca;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies.Custom;
using static Bot.Engine.TradingEngine;

namespace FormRunner.RunFiles;

public class OlsPairsTrade
{
    public void Run()
    {
        var engine = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                start: new DateTime(2000, 1, 1),
                end: DateTime.Now,
                universe: new() { "GLD", "GDX" },
                shouldWriteCsv: true))
            .WithDataSource(new AlpacaDataSource())
            .WithStrategy(new OlsPairsTradeStrategy("GLD", "GDX", 60), 1.0)
            .Build();

        RunResult result = engine.RunAsync().Result;
        var form = new BacktestResultForm(result);
        form.Show();
    }
}
