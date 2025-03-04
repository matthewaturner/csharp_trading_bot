
using Bot;
using Bot.Analyzers;
using Bot.Brokers.BackTest;
using Bot.DataSources.Csv;
using Bot.Engine;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using Bot.Models.Results;
using Bot.Strategy;

namespace FormRunner.RunFiles;

public class BuyAndHold
{
    public Form Run()
    {
        var engine = new TradingEngine
        {
            RunConfig = new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new Universe("IGE")),
            Broker = new BackTestingBroker(42.09),
            DataSource = new CsvDataSource(GlobalConfig.EpChanDataFolder),
            Analyzer = new StrategyAnalyzer(annualRiskFreeRate: .04),
            Strategy = new BuyAndHoldStrategy("IGE"),
        };

        RunResult result = engine.RunAsync().Result;

        return new ScatterPlotForm(result);
    }
}
