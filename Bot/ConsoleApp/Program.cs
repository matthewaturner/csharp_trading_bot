using Bot.Engine;
using Bot.Models;
using Bot.Logging;
using Bot.Strategies;
using Bot.Brokers.BackTest;
using Bot.Data;

var smaCrossStrat = new SMACrossoverStrategy("MSFT", 16, 64, true);
var broker = new BackTestingBroker(10000);
var dataSource = new YahooDataSource();

var engine = new TradingEngine()
{
    Logger = new ConsoleLogger(LogLevel.Verbose),
    Broker = broker,
    DataSource = dataSource,
    Strategy = smaCrossStrat,
    Symbols = new List<string> { "MSFT" }
};

await engine.RunAsync(
    RunMode.BackTest,
    TickInterval.Day,
    DateTime.Now.AddYears(-5),
    DateTime.Now);

Console.WriteLine("Done.");
