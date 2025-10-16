using System.Threading.Tasks;
using Bot;
using Bot.DataSources.Csv;
using Bot.Models.Engine;
using Bot.Models.Results;
using UX.Views;

namespace UX.RunFiles;

public class PlotCsvData
{
    public async void Run()
    {
        var source = new CsvDataSource(GlobalConfig.EpChanDataFolder);

        var bars = await source.GetHistoricalBarsAsync("IGE", Interval.OneDay, DateTime.MinValue, DateTime.MaxValue);

        // form a runresult from the raw bars
        var runResult = new RunResult(new List<string>() { "IGE" })
        {
            Returns = [.. bars.Select(b => b.AdjClose)]
        };
        var window = new BacktestResultWindow(runResult);
        window.Show();
    }
}
