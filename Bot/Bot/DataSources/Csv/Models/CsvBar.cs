
using Bot.Models;
using CsvHelper.Configuration.Attributes;
using System;

namespace Bot.DataSources.Csv.Models;

public record CsvBar
{
    [Name("Date")]
    public DateTime Date { get; set; }

    [Name("Open")]
    public decimal Open { get; set; }

    [Name("High")]
    public decimal High { get; set; }

    [Name("Low")]
    public decimal Low { get; set; }

    [Name("Close")]
    public decimal Close { get; set; }

    [Name("Volume")]
    public long Volume { get; set; }

    [Name("Adj Close")]
    public decimal AdjClose { get; set; }

    public Bar ToBotModel(string symbol) => new Bar(Date, symbol, Open, High, Low, Close, Volume, AdjClose);
}
