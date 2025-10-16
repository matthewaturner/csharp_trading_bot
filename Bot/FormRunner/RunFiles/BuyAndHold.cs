
using Bot.DataSources.Alpaca;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies.EpChan;
using static Bot.Engine.TradingEngine;

namespace FormRunner.RunFiles;

public class BuyAndHold
{
    public void Run()
    {
        var engine = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                start: new DateTime(2000, 1, 1),
                end: DateTime.Now,
                universe: new() { "XOM" }))
            .WithDataSource(new AlpacaDataSource())
            .WithStrategy(new Ex3_4_BuyAndHold("XOM"), 1.0)
            .Build();

        RunResult result = engine.RunAsync().Result;
        var form = new BacktestResultForm(result);
        form.Show();
    }
}
