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
        if (runResult == null || runResult.PortfolioSnapshots?.FirstOrDefault() == null)
        {
            Console.WriteLine("No data to export.");
            return;
        }

        using (var writer = new StreamWriter(filePath))
        {
            // header row
            writer.Write("Timestamp,Cash,PortfolioValue,LongPositionsValue,ShortPositionsValue,GrossExposure,NetExposure,CapitalAtRisk,Leverage,RealizedPnL,UnrealizedPnL,");
            writer.Write("Returns,CumulativeReturns,ExcessReturns,HighWaterMark,Drawdown,DrawdownDuration\n");

            for (int i = 0; i < runResult.PortfolioSnapshots.Count; i++)
            {
                var snapshot = runResult.PortfolioSnapshots[i];

                // snapshot values
                writer.Write(snapshot.Timestamp.ToString("yyyy-MM-dd HH:mm:ss") + ",");
                writer.Write(snapshot.Cash.ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(snapshot.PortfolioValue.ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(snapshot.LongPositionsValue.ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(snapshot.ShortPositionsValue.ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(snapshot.GrossExposure.ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(snapshot.NetExposure.ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(snapshot.CapitalAtRisk.ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(snapshot.Leverage.ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(snapshot.RealizedPnL.ToString(CultureInfo.InvariantCulture) + ",");
                writer.Write(snapshot.UnrealizedPnL.ToString(CultureInfo.InvariantCulture) + ",");

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
