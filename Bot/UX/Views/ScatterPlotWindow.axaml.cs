using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ScottPlot.Avalonia;

namespace UX.Views;

public partial class ScatterPlotWindow : Window
{
    public ScatterPlotWindow()
    {
        InitializeComponent();
    }

    private ScatterPlotWindow(Action<AvaPlot> plotAction, Action<TextBox>? formatAction = null, string? title = null) : this()
    {
        plotAction?.Invoke(Plot);
        formatAction?.Invoke(InfoTextBox);
        if (title != null) Title = title;

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

    /// <summary>
    /// DotPlot with OLS line through it.
    /// </summary>
    /// <param name="m">Slope.</param>
    /// <param name="b">Intercept.</param>
    public static ScatterPlotWindow DotPlotOLS(double[] xs, double[] ys, double m, string title)
    {
        // Generate the regression line points
        double x1 = xs.Min(); double y1 = m * x1;
        double x2 = xs.Max(); double y2 = m * x2;

        return new ScatterPlotWindow(
            plot =>
            {
                // plot the points
                var scatter = plot.Plot.Add.Scatter(xs, ys);
                scatter.LineWidth = 0;

                // plot the regression line
                var line = plot.Plot.Add.Line(x1, y1, x2, y2);
                line.Color = ScottPlot.Color.FromHex("#FF0000");
            },
            tb => tb.Text = $"Slope: {m}",
            title);
    }

    /// <summary>
    /// Simple returns over time chart.
    /// </summary>
    public static ScatterPlotWindow ReturnsOverTime(double[] xs, double[] ys, string title)
    {
        return new ScatterPlotWindow(
            plot => plot.Plot.Add.Scatter(xs, ys),
            tb => tb.Text = "Returns over time",
            title);
    }
}
