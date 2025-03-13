// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Results;
using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Bot.Helpers;

public class CsvExporter
{
    public static void ExportToCSV(RunResult runResult, string filePath)
    {
        // Ensure data is not empty
        if (runResult == null || runResult.Returns?.FirstOrDefault() == null)
        {
            Console.WriteLine("No data to export.");
            return;
        }

        using (var writer = new StreamWriter(filePath))
        {
            // header row
            writer.Write($"Timestamp,");
            foreach (string symbol in runResult.SymbolUniverse)
            {
                writer.Write($"{symbol},{symbol}_Return,");
            }
            writer.Write("Returns,CumulativeReturns,ExcessReturns,HighWaterMark,Drawdown,DrawdownDuration\n");

            // for every timestamp
            for (int i = 0; i < runResult.Returns.Count; i++)
            {
                writer.Write(runResult.Timestamps[i].ToString("yyyy-MM-ddT HH:mm") + ",");
                foreach (string symbol in runResult.SymbolUniverse)
                {
                    writer.Write(runResult.UnderlyingPrices[symbol][i] + ",");
                    writer.Write(runResult.UnderlyingReturns[symbol][i] + ",");
                }

                // calculated values
                writer.Write(runResult.Returns[i].ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(runResult.CumulativeReturns[i].ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(runResult.ExcessReturns[i].ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(runResult.HighWaterMark[i].ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(runResult.Drawdown[i].ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(runResult.DrawdownDuration[i].ToString(CultureInfo.InvariantCulture) + "\n");
            }

            // summary metrics
            writer.WriteLine();
            writer.WriteLine("AnnualizedSharpeRatio," + runResult.AnnualizedSharpeRatio);
            writer.WriteLine("MaximumDrawdown," + runResult.MaximumDrawdown);
            writer.WriteLine("MaximumDrawdownDuration," + runResult.MaximumDrawdownDuration);
        }

        Console.WriteLine($"CSV exported to {filePath}");
    }
}
