using Bot;
using Bot.DataSources.Csv;
using Bot.Models;
using FormRunner;

namespace Runner.RunFiles;

public class PlotCsvData
{
    public Form Run()
    {
        var source = new CsvDataSource(GlobalConfig.EpChanDataFolder);

        var bars = source.GetHistoricalBarsAsync("IGE", Interval.OneDay, DateTime.MinValue, DateTime.MaxValue).Result;

        var form = new ScatterPlotForm(
            bars.Select(b => b.Timestamp).ToArray(),
            bars.Select(b => (decimal)b.AdjClose!).ToArray());

        return form;
    }
}
