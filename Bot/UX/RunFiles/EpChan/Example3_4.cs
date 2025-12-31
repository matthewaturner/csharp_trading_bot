using System.Threading.Tasks;
using Bot;
using Bot.Brokers.Backtest;
using Bot.DataSources.Csv;
using Bot.Models.Broker;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies.EpChan;
using Microsoft.Extensions.Logging;
using UX.Views;
using static Bot.Engine.TradingEngine;

namespace UX.RunFiles.EpChan;

public class Example3_4
{
    public async void Run()
    {
        Console.WriteLine("Testing");

        var broker = new BacktestBroker(100000, ExecutionMode.OnCurrentBarClose);
        var engine = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new() { "IGE" },
                minLogLevel: LogLevel.Debug,
                shouldWriteCsv: true,
                annualRiskFreeRate: .04))
            .WithDataSource(new CsvDataSource(GlobalConfig.EpChanDataFolder))
            .WithStrategy(new Ex3_4_BuyAndHold("IGE"), 1.0)
            .WithExecutionEngine(broker)
            .Build();

        RunResult result = await engine.RunAsync();
        var window = new BacktestResultWindow(result);
        window.Show();
    }
}
