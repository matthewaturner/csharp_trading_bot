# UX - Avalonia UI Trading Bot Runner

This is a cross-platform UI application built with Avalonia UI, converted from the Windows Forms-based FormRunner project. It provides interactive visualizations for trading strategy backtests.

## Features

- **Cross-platform**: Runs on Windows, macOS, and Linux (unlike the original Windows Forms version)
- **ScottPlot Integration**: Uses ScottPlot.Avalonia for charting and data visualization
- **Backtest Results Viewer**: Displays cumulative returns, Sharpe ratio, max drawdown, and other metrics
- **Scatter Plot Viewer**: Shows OLS regression lines, spread returns, and other scatter plots

## Usage

Run a specific backtest file by using the `--fileName` argument:

```bash
dotnet run --project Bot/UX -- --fileName Example3_4
```

Available run files:
- `BuyAndHold` - Simple buy and hold strategy
- `OlsPairsTrade` - Pairs trading using OLS regression
- `PlotCsvData` - Plot data from CSV files
- `Example3_4` - Buy and hold example from Quantitative Trading by E.P. Chan
- `Example3_5` - Long-short example from Quantitative Trading
- `Example3_6` - OLS pairs trading example from Quantitative Trading

## Project Structure

```
UX/
├── App.axaml              - Application XAML definition
├── App.axaml.cs           - Application code-behind
├── Program.cs             - Entry point with run file loader
├── UX.csproj              - Project file
├── Views/                 - UI Windows
│   ├── BacktestResultWindow.axaml
│   ├── BacktestResultWindow.axaml.cs
│   ├── ScatterPlotWindow.axaml
│   └── ScatterPlotWindow.axaml.cs
└── RunFiles/              - Runnable backtest examples
    ├── BuyAndHold.cs
    ├── OlsPairsTrade.cs
    ├── PlotCsvData.cs
    └── EpChan/
        ├── Example3_4.cs
        ├── Example3_5.cs
        └── Example3_6.cs
```

## Windows

### BacktestResultWindow
Displays:
- Cumulative returns plot over time
- Sharpe ratio
- Maximum drawdown
- Maximum drawdown duration

### ScatterPlotWindow
Displays:
- Scatter plots with optional regression lines
- Customizable info panel
- Time series data

## Development

To add a new run file:
1. Create a new class in `RunFiles/` directory
2. Implement a `Run()` method that creates and shows a window
3. Reference the class name when running: `dotnet run -- --fileName YourClassName`

Example:
```csharp
namespace UX.RunFiles;

public class MyBacktest
{
    public void Run()
    {
        // Your backtest logic here
        var result = engine.RunAsync().Result;
        var window = new BacktestResultWindow(result);
        window.Show();
    }
}
```

## Dependencies

- Avalonia 11.2.2
- ScottPlot.Avalonia 5.0.54
- Bot project (trading engine and strategies)
