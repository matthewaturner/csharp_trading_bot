// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot;
using Bot.DataSources.Csv;
using Bot.Models.Engine;

namespace FormRunner.RunFiles.EpChan;

public class Example3_6
{
    public Form Run()
    {
        var dataSource = new CsvDataSource(GlobalConfig.EpChanDataFolder);
        var gld = dataSource.GetHistoricalBarsAsync("GLD", Interval.OneDay, DateTime.MinValue, DateTime.MaxValue).Result;
        var gdx = dataSource.GetHistoricalBarsAsync("GDX", Interval.OneDay, DateTime.MinValue, DateTime.MaxValue).Result;

        // take the union
        var sharedTimestamps = gld.Select(b => b.Timestamp).Intersect(gdx.Select(b => b.Timestamp)).ToList();
        var gldFiltered = gld.Where(b => sharedTimestamps.Contains(b.Timestamp)).ToList();
        var gdxFiltered = gdx.Where(b => sharedTimestamps.Contains(b.Timestamp)).ToList();

        var gldTrainSet = gldFiltered.Take(252).Select(b => b.AdjClose).ToArray();
        var gdxTrainSet = gdxFiltered.Take(252).Select(b => b.AdjClose).ToArray();

        var ols = new OLSForm(gldTrainSet, gdxTrainSet);
        ols.Show();

        return ols;
    }
}
