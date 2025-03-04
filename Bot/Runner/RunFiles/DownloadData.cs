using Bot.DataSources.Alpaca;
using Bot.Models.Engine;

namespace Runner.RunFiles;

public class DownloadData
{
    public void Run()
    {
        AlpacaDataSource source = new AlpacaDataSource();

        var start = new DateTime(2015, 1, 1);
        var end = new DateTime(2025, 2, 15);
        var bars = source.GetHistoricalBarsAsync("MSFT", Interval.OneDay, start, end).Result;

        foreach (var bar in bars)
        {
            Console.WriteLine(bar.ToString());
        }
    }
}
