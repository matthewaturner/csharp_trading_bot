
using Bot;
using Bot.Brokers.BackTest;
using Bot.DataSources.Alpaca;
using Bot.DataSources.Csv;
using Bot.Engine;
using Bot.Models;
using Bot.Models.Results;
using Bot.Strategy;

namespace FormRunner.RunFiles;

public class BuyAndHold
{
    public Form Run()
    {
        var smaCrossStrat = new BuyAndHoldStrategy();
        var broker = new BackTestingBroker(10000);
        var dataSource = new CsvDataSource(GlobalConfig.EpChanDataFolder);

        var engine = new TradingEngine()
        {
            Broker = broker,
            DataSource = dataSource,
            Strategy = smaCrossStrat,
            Symbols = ["IGE"]
        };

        RunResult result = engine.RunAsync(RunMode.BackTest, Interval.OneDay).Result;

        return new ScatterPlotForm(result);
    }
}
