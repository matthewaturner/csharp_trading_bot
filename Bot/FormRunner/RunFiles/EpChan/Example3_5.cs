// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

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

public class Example3_5
{
    public Form Run()
    {
        var engine = new TradingEngine
        {
            RunConfig = new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                universe: new Universe("IGE", "SPY"),
                minLogLevel: LogLevel.Debug,
                shouldWriteCsv: true),
            Broker = new BacktestBroker(10000),
            DataSource = new CsvDataSource(GlobalConfig.EpChanDataFolder),
            Analyzer = new StrategyAnalyzer(), // no risk free rate for fully invested long/short strategy
            Strategy = new Ex3_5_LongShort("IGE", "SPY"),
        };

        RunResult result = engine.RunAsync().Result;

        return new ScatterPlotForm(result);
    }
}
