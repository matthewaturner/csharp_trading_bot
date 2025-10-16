
using Bot.DataSources.Csv.Models;
using Bot.Models.Engine;
using Bot.Models.MarketData;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.DataSources.Csv;

/// <summary>
/// Data source for reading csv files from the data/ folder.
/// </summary>
public class CsvDataSource : DataSourceBase
{
    private readonly string DataPath;

    public CsvDataSource(string dataFolderPath)
    {
        DataPath = dataFolderPath;
    }

    public override Task<List<Bar>> GetHistoricalBarsAsync(string symbol, Interval interval, DateTime start, DateTime end)
    {
        string fileName = Path.Combine(DataPath, $"{symbol}_{interval}.csv");

        if (!File.Exists(fileName))
            throw new FileNotFoundException($"CSV file not found: {fileName}");

        using var reader = new StreamReader(fileName);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true });

        var records = csv.GetRecords<CsvBar>()
            .Where(b => b.Date >= start && b.Date <= end)
            .OrderBy(b => b.Date)
            .ToList();

        return Task.FromResult(records.Select(b => b.ToBotModel(symbol)).ToList());
    }
}

