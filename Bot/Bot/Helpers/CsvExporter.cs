using Bot.Models.Results;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bot.Helpers;

public class CsvExporter
{
    public static void ExportToCSV(RunResult runResult, string filePath)
    {
        // Ensure data is not empty
        if (runResult == null)
        {
            Console.WriteLine("No data to export.");
            return;
        }

        // Get all properties of RunResult that are of type List<DatedValue>
        var datedValueFields = runResult.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => f.FieldType == typeof(List<DatedValue>))
            .ToList();

        // Ensure there's at least one property to export
        if (datedValueFields.Count == 0)
        {
            Console.WriteLine("No DatedValue lists found.");
            return;
        }

        // Flatten all timestamps from all lists to create a unique, ordered set of timestamps
        var allTimestamps = datedValueFields
            .SelectMany(field => ((List<DatedValue>)field.GetValue(runResult)).Select(dv => dv.Timestamp))
            .Distinct()
            .OrderBy(t => t)
            .ToList();

        // Dictionary to store values by timestamp
        var dataDict = allTimestamps.ToDictionary(t => t, t => new Dictionary<string, double?>());

        // Populate dictionary with values from each list
        foreach (var field in datedValueFields)
        {
            string columnName = field.Name;
            var datedValues = (List<DatedValue>)field.GetValue(runResult);

            foreach (var dv in datedValues)
            {
                if (dataDict.ContainsKey(dv.Timestamp))
                {
                    dataDict[dv.Timestamp][columnName] = dv.Value;
                }
            }
        }

        // Write to CSV
        using (var writer = new StreamWriter(filePath))
        {
            // Write header row
            writer.WriteLine("Timestamp," + string.Join(",", datedValueFields.Select(f => f.Name)));

            // Write data rows
            foreach (var timestamp in allTimestamps)
            {
                var rowValues = datedValueFields
                    .Select(f => dataDict[timestamp].TryGetValue(f.Name, out var value) ? value?.ToString(CultureInfo.InvariantCulture) ?? "" : "");
                writer.WriteLine($"{timestamp:yyyy-MM-dd HH:mm:ss},{string.Join(",", rowValues)}");
            }
        }

        Console.WriteLine($"CSV exported to {filePath}");
    }
}
