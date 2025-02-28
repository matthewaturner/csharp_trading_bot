using Bot;
using Bot.DataSources.Csv;
using Bot.Models;
using Bot.Models.Results;

namespace FormRunner.RunFiles;

public class PlotCsvData
{
    public Form Run()
    {
        var source = new CsvDataSource(GlobalConfig.EpChanDataFolder);

        var bars = source.GetHistoricalBarsAsync("IGE", Interval.OneDay, DateTime.MinValue, DateTime.MaxValue).Result;

        // form a runresult from the raw bars
        var runResult = new RunResult() { 
            PortfolioValues = bars.Select(b => new DatedValue(b.Timestamp, (double)b.Close)).ToList() 
        };
        var form = new ScatterPlotForm(runResult);

        return form;
    }
}
