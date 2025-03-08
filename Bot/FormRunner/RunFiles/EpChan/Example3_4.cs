
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
                minLogLevel: LogLevel.Debug,
                shouldWriteCsv: true),
            Broker = new BacktestBroker(42.09),
            DataSource = new CsvDataSource(GlobalConfig.EpChanDataFolder),
            Analyzer = new StrategyAnalyzer(annualRiskFreeRate: .04),
            Strategy = new Ex3_4_BuyAndHold("IGE"),
        };

        RunResult result = engine.RunAsync().Result;

        return new ScatterPlotForm(result);
    }
}
