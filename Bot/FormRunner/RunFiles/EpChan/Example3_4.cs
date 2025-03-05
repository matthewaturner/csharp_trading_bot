
using Bot;
using Bot.Analyzers;
using Bot.Brokers.BackTest;
using Bot.DataSources.Csv;
using Bot.Engine;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using Bot.Models.Results;
using Bot.Strategy;
using Microsoft.Extensions.Logging;

namespace FormRunner.RunFiles.EpChan;

public class Example3_4
{
    public Form Run()
    {
        var engine = new TradingEngine
        {
            RunConfig = new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new Universe("IGE"),
                minLogLevel: LogLevel.Debug),
            Broker = new BackTestingBroker(42.09),
            DataSource = new CsvDataSource(GlobalConfig.EpChanDataFolder),
            Analyzer = new StrategyAnalyzer(annualRiskFreeRate: .04),
            Strategy = new BuyAndHoldStrategy("IGE"),
        };

        RunResult result = engine.RunAsync().Result;

        return new ScatterPlotForm(result);
    }
}
