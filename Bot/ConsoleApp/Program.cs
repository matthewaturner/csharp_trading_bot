using Bot.Engine;
using Bot.Models;
using Bot.Strategies;
using Bot.Brokers.BackTest;
using Bot.Data;

var smaCrossStrat = new SMACrossoverStrategy("MSFT", 50, 200, true);
var broker = new BackTestingBroker(10000);
var dataSource = new YahooDataSource();

var engine = new TradingEngine()
{
    Broker = broker,
    DataSource = dataSource,
    Strategy = smaCrossStrat,
    Symbols = new List<string> { "MSFT" }
};

await engine.RunAsync(
    RunMode.BackTest,
    TickInterval.Day,
    DateTime.Now.AddDays(-365),
    DateTime.Now);

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
