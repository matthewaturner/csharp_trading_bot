using System.Collections.Generic;

namespace Theo.DataSources.Alpaca.Models;

public class BarsResponse
{
    public List<Bar> Bars { get; set; } = new();
}