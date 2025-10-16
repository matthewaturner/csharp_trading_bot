using System.Threading.Tasks;
using Bot;
using Bot.DataSources.Csv;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies.EpChan;
using Microsoft.Extensions.Logging;
using UX.Views;
using static Bot.Engine.TradingEngine;

namespace UX.RunFiles.EpChan;

public class Example3_5
{
    public async void Run()
    {
        var engine = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new() { "IGE", "SPY" },
                minLogLevel: LogLevel.Debug,
                shouldWriteCsv: true))
            .WithDataSource(new CsvDataSource(GlobalConfig.EpChanDataFolder))
            .WithStrategy(new Ex3_5_LongShort("IGE", "SPY"), 1.0)
            .Build();

        RunResult result = await engine.RunAsync();
        var window = new BacktestResultWindow(result);
        window.Show();
    }
}
