
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
        var runResult = new RunResult(new List<string>() { "IGE" })
        {
            Returns = [.. bars.Select(b => b.AdjClose)]
        };
        var form = new BacktestResultForm(runResult);

        return form;
    }
}
