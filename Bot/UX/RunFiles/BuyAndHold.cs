using System.Threading.Tasks;
using Bot.DataSources.Alpaca;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies.EpChan;
using UX.Views;
using static Bot.Engine.TradingEngine;

namespace UX.RunFiles;

public class BuyAndHold
{
    public async void Run()
    {
        Console.WriteLine("Starting BuyAndHold backtest...");
        
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

        Console.WriteLine("Running backtest...");
        RunResult result = await engine.RunAsync();
        
        Console.WriteLine("Creating window...");
        var window = new BacktestResultWindow(result);
        
        Console.WriteLine("Showing window...");
        window.Show();
        
        Console.WriteLine("Window shown successfully.");
    }
}
