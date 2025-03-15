// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot;
using Bot.DataSources.Csv;
using Bot.Helpers;
using Bot.Models.Engine;
using Bot.Strategies.EpChan;
using Microsoft.Extensions.Logging;
using static Bot.Engine.TradingEngine;

namespace FormRunner.RunFiles.EpChan;

public class Example3_6
{
    public void Run()
    {
        // format the train set data
        var dataSource = new CsvDataSource(GlobalConfig.EpChanDataFolder);
        var gld = dataSource.GetHistoricalBarsAsync("GLD", Interval.OneDay, DateTime.MinValue, DateTime.MaxValue).Result;
        var gdx = dataSource.GetHistoricalBarsAsync("GDX", Interval.OneDay, DateTime.MinValue, DateTime.MaxValue).Result;

        var sharedTimestamps = gld.Select(b => b.Timestamp).Intersect(gdx.Select(b => b.Timestamp)).ToList();
        var gldFiltered = gld.Where(b => sharedTimestamps.Contains(b.Timestamp)).ToList();
        var gdxFiltered = gdx.Where(b => sharedTimestamps.Contains(b.Timestamp)).ToList();

        var gldTrainSet = gldFiltered.Take(252).Select(b => b.AdjClose).ToArray();
        var gdxTrainSet = gdxFiltered.Take(252).Select(b => b.AdjClose).ToArray();

        // calculate the OLS regression coefficients
        double m = MathFunctions.OLS(gdxTrainSet, gldTrainSet);
        var olsForm = ScatterPlotForm.DotPlotOLS(gdxTrainSet, gldTrainSet, m, "OLS: x_axis=GDX y_axis=GLD");
        olsForm.Show();

        // calculate the spread of the portfolio formed using the hedge ratio we calculated
        var spread = gldTrainSet.Zip(gdxTrainSet, (gld, gdx) => gld - m * gdx).ToArray();
        var sharedTimestampsOADate = sharedTimestamps.Select(t => t.ToOADate()).ToArray();
        var spreadForm = ScatterPlotForm.ReturnsOverTime(sharedTimestampsOADate, spread, "Spread Returns: GLD - m*GDX");
        spreadForm.Show();

        // run backtest on the training set
        double spreadMean = spread.Average();
        double spreadStdDev = MathFunctions.StdDev(spread);

        var trainResult = new EngineBuilder()
            .WithConfig(new RunConfig(
                interval: Interval.OneDay,
                runMode: RunMode.BackTest,
                start: sharedTimestamps.First(),
                end: sharedTimestamps.Last().AddDays(1),
                universe: new() { "GLD", "GDX" },
                minLogLevel: LogLevel.Debug,
                shouldWriteCsv: true))
            .WithDataSource(new CsvDataSource(GlobalConfig.EpChanDataFolder))
            .WithStrategy(new Ex3_6_OlsPairsTrade("GLD", "GDX", m, spreadMean, spreadStdDev), 1.0)
            .Build().RunAsync().Result;
        var trainResultform = new BacktestResultForm(trainResult);
        trainResultform.Show();
    }
}
