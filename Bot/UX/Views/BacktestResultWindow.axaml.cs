using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Bot.Models.Results;
using ScottPlot.Avalonia;

namespace UX.Views;

public partial class BacktestResultWindow : Window
{
    public BacktestResultWindow()
    {
        InitializeComponent();
    }

    public BacktestResultWindow(RunResult runResult, [CallerMemberName] string? name = "Run Result") : this()
    {
        Title = name;

        // Add scatter plot with cumulative returns
        Plot.Plot.Add.Scatter(
            runResult.Timestamps.Select(v => v.ToOADate()).ToArray(),
            runResult.CumulativeReturns.ToArray());
        Plot.Plot.Axes.DateTimeTicksBottom();
        Plot.Refresh();

        // Set statistics values
        SharpeRatioValue.Text = runResult.AnnualizedSharpeRatio.ToString();
        MaxDrawdownValue.Text = runResult.MaximumDrawdown.ToString();
        MaxDrawdownDurationValue.Text = runResult.MaximumDrawdownDuration.ToString();

        Closing += (sender, e) =>
        {
            Environment.Exit(0);
        };
    }

    public new void Show()
    {
        // Set this as the main window for the application
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = this;
        }
        base.Show();
    }
}
