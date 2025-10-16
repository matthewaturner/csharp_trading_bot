using System.Threading.Tasks;
using Bot.DataSources.Alpaca;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies.Custom;
using UX.Views;
using static Bot.Engine.TradingEngine;

namespace UX.RunFiles;

public class OlsPairsTrade
{
    public async void Run()
    {
        var engine = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                start: new DateTime(2021, 1, 1),
                end: DateTime.Now,
                universe: new() { "GLD", "GDX" },
                shouldWriteCsv: true))
            .WithDataSource(new AlpacaDataSource())
            .WithStrategy(new OlsPairsTradeStrategy("GLD", "GDX", 60), 1.0)
            .Build();

        RunResult result = await engine.RunAsync();
        var window = new BacktestResultWindow(result);
        window.Show();
    }
}
