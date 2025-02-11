﻿using Theo.DataSources.Alpaca;

namespace Runner.RunFiles;

public class DownloadData
{
    public void Run()
    {
        AlpacaDataSource source = new AlpacaDataSource();

        var bars = source.GetHistoricalBarsAsync("MSFT", "1D", "2024-01-01", "2025-01-01").Result;

        foreach (var bar in bars)
        {
            Console.WriteLine($"{bar.T} {bar.O} {bar.H} {bar.L} {bar.C} {bar.V}");
        }
    }
}
