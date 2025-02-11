﻿
using Theo.Brokers.BackTest;
using Theo.Data;
using Theo.Engine;
using Theo.Logging;
using Theo.Models;
using Theo.Strategies;

namespace Runner.RunFiles;

public class SmaCrossover_Example
{
    public static void Run()
    {
        var smaCrossStrat = new SMACrossoverStrategy("MSFT", 16, 64, true);
        var broker = new BackTestingBroker(10000);
        var dataSource = new YahooDataSource();

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
            DataInterval.Day,
            DateTime.Now.AddYears(-5),
            DateTime.Now).RunSynchronously();

        Console.WriteLine("Done.");
    }
}
