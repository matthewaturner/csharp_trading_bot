using System.Threading.Tasks;
using Bot.Brokers.Backtest;
using Bot.DataSources.Alpaca;
using Bot.Models.Broker;
using Bot.Models.Engine;
using Bot.Models.Results;
using Bot.Strategies.EpChan;
using Microsoft.Extensions.Logging;
using UX.Views;
using static Bot.Engine.TradingEngine;

namespace UX.RunFiles;

public class BuyAndHold
{
    public async void Run()
    {
        Console.WriteLine("Starting BuyAndHold backtest...");

        try
        {
            var broker = new BacktestBroker(100000, ExecutionMode.OnCurrentBarClose);
            var engine = new EngineBuilder()
                .WithConfig(new RunConfig(
                    interval: Interval.OneDay,
                    runMode: RunMode.BackTest,
                    start: new DateTime(2020, 1, 1),
                    end: DateTime.Now,
                    universe: new() { "XOM" },
                    logLevel: LogLevel.Information))
                .WithDataSource(new AlpacaDataSource())
                .WithStrategy(new Ex3_4_BuyAndHold("XOM"), 1.0)
                .WithExecutionEngine(broker, rebalanceThreshold: 0.01)
                .Build();

            Console.WriteLine("Running backtest...");
            RunResult result = await engine.RunAsync();
            
            Console.WriteLine("Creating window...");
            var window = new BacktestResultWindow(result);
            
            Console.WriteLine("Showing window...");
            window.Show();
            
            Console.WriteLine("Window shown successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
            Environment.Exit(1);
        }
    }
}
