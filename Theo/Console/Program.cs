using Theo.Engine;
using Theo.Models;
using Theo.Logging;
using Theo.Strategies;
using Theo.Brokers.BackTest;
using Theo.Data;

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
