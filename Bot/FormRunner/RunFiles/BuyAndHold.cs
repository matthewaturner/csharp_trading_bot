
using Bot.Analyzers;
using Bot.Brokers.BackTest;
using Bot.DataSources.Alpaca;
using Bot.Engine;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using Bot.Models.Results;
using Bot.Strategy;
using Bot.Strategy.EpChan;

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
                start: new DateTime(2000, 1, 1),
                end: DateTime.Now,
                universe: new Universe("XOM")),
            Broker = new BackTestingBroker(42.09),
            DataSource = new AlpacaDataSource(),
            Analyzer = new StrategyAnalyzer(annualRiskFreeRate: .04),
            Strategy = new Ex3_4_BuyAndHold("XOM"),
        };

        RunResult result = engine.RunAsync().Result;

        return new ScatterPlotForm(result);
    }
}
