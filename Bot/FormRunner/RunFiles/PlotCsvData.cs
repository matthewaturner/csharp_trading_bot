// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot;
using Bot.DataSources.Csv;
using Bot.Models.Engine;
using Bot.Models.Results;

namespace FormRunner.RunFiles;

public class PlotCsvData
{
    public Form Run()
    {
        var source = new CsvDataSource(GlobalConfig.EpChanDataFolder);

        var bars = source.GetHistoricalBarsAsync("IGE", Interval.OneDay, DateTime.MinValue, DateTime.MaxValue).Result;

        // form a runresult from the raw bars
        var runResult = new RunResult()
        {
            PortfolioSnapshots = bars.Select(b => new PortfolioSnapshot()
            {
                Timestamp = b.Timestamp,
                PortfolioValue = b.AdjClose,
            }).ToList()
        };
        var form = new ScatterPlotForm(runResult);

        return form;
    }
}
