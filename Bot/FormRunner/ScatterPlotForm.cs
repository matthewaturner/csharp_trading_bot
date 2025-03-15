// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using ScottPlot.WinForms;

namespace FormRunner;

public partial class ScatterPlotForm : Form
{
    private ScatterPlotForm(Action<FormsPlot> plotAction, Action<RichTextBox>? formatAction = null, string? title = null)
    {
        InitializeComponent();
        plotAction?.Invoke(formsPlot1);
        formatAction?.Invoke(richTextBox1);
        if (title != null) this.Text = title;

        this.FormClosing += OnAnyFormClosing;
    }

    private void OnAnyFormClosing(object? sender, FormClosingEventArgs e)
    {
        Application.Exit();
    }

    /// <summary>
    /// DotPlot with OLS line through it.
    /// </summary>
    /// <param name="m">Slope.</param>
    /// <param name="b">Intercept.</param>
    public static ScatterPlotForm DotPlotOLS(double[] xs, double[] ys, double m, string title)
    {
        // Generate the regression line points
        double x1 = xs.Min(); double y1 = m * x1;
        double x2 = xs.Max(); double y2 = m * x2;

        return new ScatterPlotForm(
            plot =>
            {
                // plot the points
                var scatter = plot.Plot.Add.Scatter(xs, ys);
                scatter.LineWidth = 0;

                // plot the regression line
                var line = plot.Plot.Add.Line(x1, y1, x2, y2);
                line.Color = ScottPlot.Color.FromColor(Color.Red);
            },
            rtb => rtb.AppendText($"Slope: {m}"),
            title);
    }

    /// <summary>
    /// Simple returns over time chart.
    /// </summary>
    public static ScatterPlotForm ReturnsOverTime(double[] xs, double[] ys, string title)
    {
        return new ScatterPlotForm(
            plot => plot.Plot.Add.Scatter(xs, ys),
            rtb => rtb.AppendText("Returns over time"),
            title);
    }
}
