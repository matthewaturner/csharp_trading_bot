
using Bot.Brokers.BackTest;
using Bot.DataSources.Alpaca;
using Bot.Engine;
using Bot.Models;
using Bot.Models.Results;
using Bot.Strategy;

namespace Runner.RunFiles;

public class BuyAndHold
{
    public void Run()
    {
        var smaCrossStrat = new BuyAndHoldStrategy();
        var broker = new BackTestingBroker(10000);
        var dataSource = new AlpacaDataSource();

        var engine = new TradingEngine()
        {
            Broker = broker,
            DataSource = dataSource,
            Strategy = smaCrossStrat,
            Symbols = ["MSFT"]
        };

        RunResult result = engine.RunAsync(
            RunMode.BackTest,
            Interval.OneDay,
            DateTime.Now.AddYears(-5),
            DateTime.Now).Result;

        Console.WriteLine("Done.");
    }
}
