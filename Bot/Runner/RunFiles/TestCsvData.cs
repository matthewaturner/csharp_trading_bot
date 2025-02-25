using Bot;
using Bot.DataSources.Csv;
using Bot.Models;

namespace Runner.RunFiles;

public class TestCsvData
{
    public void Run()
    {
        var source = new CsvDataSource(GlobalConfig.EpChanDataFolder);

        var bars = source.GetHistoricalBarsAsync("IGE", Interval.OneDay, DateTime.MinValue, DateTime.MaxValue).Result;

        foreach (var bar in bars)
        {
            Console.WriteLine(bar.ToString());
        }
    }
}
