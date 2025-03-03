
using Bot;
using Bot.Analyzers;
using Bot.Brokers.BackTest;
using Bot.DataSources.Csv;
using Bot.Engine;
using Bot.Models;
using Bot.Models.Results;
using Bot.Strategy;

namespace IntegrationTests.EpChanExamples;

public class EpChanTests
{
    [Test]
    public void Example3_4()
    {
        var smaCrossStrat = new BuyAndHoldStrategy();
        var broker = new BackTestingBroker(42.09);
        var dataSource = new CsvDataSource(GlobalConfig.EpChanDataFolder);
        var analyzer = new StrategyAnalyzer(annualRiskFreeRate: .04);

        var engine = new TradingEngine()
        {
            Broker = broker,
            DataSource = dataSource,
            Analyzer = analyzer,
            Strategy = smaCrossStrat,
            Symbol = "IGE"
        };

        RunResult result = engine.RunAsync(RunMode.BackTest, Interval.OneDay).Result;

        result.AnnualizedSharpeRatio.IsApproximately(0.789054m);
    }
}