using Bot.Models.Engine;
using Bot.Models.Results;
using UX.Views;
using static Bot.Engine.TradingEngine;

namespace UX.RunFiles;

public class TestWindow
{
    public void Run()
    {
        Console.WriteLine("Creating test window...");
        
        // Create a simple test result with proper initialization
        var testResult = new RunResult(new List<string> { "TEST" });
        
        // Populate with test data
        testResult.Timestamps.AddRange(new[]
        {
            DateTime.Now.AddDays(-4),
            DateTime.Now.AddDays(-3),
            DateTime.Now.AddDays(-2),
            DateTime.Now.AddDays(-1),
            DateTime.Now
        });
        
        testResult.UnderlyingPrices["TEST"].AddRange(new[] { 100.0, 110.0, 105.0, 115.0, 120.0 });
        testResult.SymbolWeights["TEST"].AddRange(new[] { 1.0, 1.0, 1.0, 1.0, 1.0 });
        
        // Calculate results
        testResult.CalculateResults(0.04, Interval.OneDay);
        
        Console.WriteLine("Creating BacktestResultWindow...");
        var window = new BacktestResultWindow(testResult, "Test Window");
        
        Console.WriteLine("Calling Show()...");
        window.Show();
        
        Console.WriteLine("Window shown.");
    }
}
