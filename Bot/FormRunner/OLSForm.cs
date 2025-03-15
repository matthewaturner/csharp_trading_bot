// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using System.Data;

namespace FormRunner;

public partial class OLSForm : Form
{
    public OLSForm(double[] xs, double[] ys)
    {
        InitializeComponent();

        var scatter = formsPlot1.Plot.Add.Scatter(xs, ys);
        scatter.LineWidth = 0;

        // Calculate the OLS regression coefficients
        var (m, b) = CalculateOLS(xs, ys);

        // Generate the regression line points
        double x1 = xs.Min();
        double y1 = m * x1 + b;
        double x2 = xs.Max();
        double y2 = m * x2 + b;

        // Plot the regression line
        var line = formsPlot1.Plot.Add.Line(x1, y1, x2, y2);
        line.Color = ScottPlot.Color.FromColor(Color.Red);

        // Set the labels for the slope and intercept
        slopeValue.Text = m.ToString();
        intersectValue.Text = b.ToString();

        // Refresh the plot
        formsPlot1.Refresh();
    }

    public (double m, double b) CalculateOLS(double[] xs, double[] ys)
    {
        if (xs.Length != ys.Length)
            throw new ArgumentException("The length of xs and ys must be the same.");

        int n = xs.Length;
        double sumX = xs.Sum();
        double sumY = ys.Sum();
        double sumX2 = xs.Select(x => x * x).Sum();
        double sumXY = xs.Zip(ys, (x, y) => x * y).Sum();

        double m = (n * sumXY - sumX * sumY) / (n * sumX2 - sumX * sumX);
        double b = (sumY - m * sumX) / n;

        return (m, b);
    }

    private void label1_Click(object sender, EventArgs e)
    {

    }
}
