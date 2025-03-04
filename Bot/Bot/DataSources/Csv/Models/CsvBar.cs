using Bot.Models.MarketData;
using CsvHelper.Configuration.Attributes;
using System;

namespace Bot.DataSources.Csv.Models;

public record CsvBar
{
    [Name("Date")]
    public DateTime Date { get; set; }

    [Name("Open")]
    public double Open { get; set; }

    [Name("High")]
    public double High { get; set; }

    [Name("Low")]
    public double Low { get; set; }

    [Name("Close")]
    public double Close { get; set; }

    [Name("Volume")]
    public long Volume { get; set; }

    [Name("Adj Close")]
    public double AdjClose { get; set; }

    public Bar ToBotModel(string symbol) => new Bar(Date, symbol, Open, High, Low, Close, Volume, AdjClose);
}
