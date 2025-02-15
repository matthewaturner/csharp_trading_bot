
using Bot.Brokers.BackTest;
using Bot.DataSources.Alpaca;
using Bot.Engine;
using Bot.Logging;
using Bot.Models;
using Bot.Strategies;

namespace Runner.RunFiles;

public class SmaCrossover_Example
{
    public static void Run()
    {
        var smaCrossStrat = new SMACrossoverStrategy("MSFT", 16, 64, true);
        var broker = new BackTestingBroker(10000);
        var dataSource = new AlpacaDataSource();

        var engine = new TradingEngine()
        {
            Logger = new ConsoleLogger(LogLevel.Verbose),
            Broker = broker,
            DataSource = dataSource,
            Strategy = smaCrossStrat,
            Symbols = ["MSFT"]
        };

        engine.RunAsync(
            RunMode.BackTest,
            Interval.OneDay,
            DateTime.Now.AddYears(-5),
            DateTime.Now).RunSynchronously();

        Console.WriteLine("Done.");
    }
}
