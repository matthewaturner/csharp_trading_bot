
using Bot.Brokers.Backtest;
using Bot.DataSources.Alpaca;
using Bot.Engine;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using Bot.Models.Results;
using Bot.Strategy;

namespace Runner.RunFiles;

public class BuyAndHold
{
    public void Run()
    {
        var engine = new TradingEngine()
        {
            RunConfig = new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                start: DateTime.Now.AddYears(-5),
                end: DateTime.Now,
                universe: new Universe("MSFT")),
            Broker = new BacktestBroker(10000),
            DataSource = new AlpacaDataSource(),
            Strategy = new BuyAndHoldStrategy("IGE"),
        };

        RunResult result = engine.RunAsync().Result;

        Console.WriteLine("Done.");
    }
}
