
using Bot;
using Bot.Analyzers;
using Bot.Brokers.Backtest;
using Bot.DataSources.Csv;
using Bot.Engine;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using Bot.Models.Results;
using Bot.Strategy.EpChan;
using Microsoft.Extensions.Logging;

namespace IntegrationTests.EpChanExamples;

public class EpChanTests
{
    [Test]
    public void Example3_4()
    {
        var engine = new TradingEngine()
        {
            RunConfig = new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new Universe("IGE"),
                minLogLevel: LogLevel.Error),
            Broker = new BacktestBroker(10000), // it should not matter how much capital you begin with
            DataSource = new CsvDataSource(GlobalConfig.EpChanDataFolder),
            Analyzer = new StrategyAnalyzer(annualRiskFreeRate: .04),
            Strategy = new Ex3_4_BuyAndHold("IGE"),
        };

        RunResult result = engine.RunAsync().Result;

        result.AnnualizedSharpeRatio.IsApproximately(0.78931753834485019m);
    }

}