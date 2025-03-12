using Bot.Models.Results;
using System.Runtime.CompilerServices;

namespace FormRunner;

public partial class ScatterPlotForm : Form
{
    public ScatterPlotForm(RunResult runResult, [CallerMemberName] string? name = "Run Result")
    {
        InitializeComponent();
        this.Text = name;

        formsPlot1.Plot.Add.Scatter(
            runResult.Timestamps.Select(v => v.ToOADate()).ToArray(),
            runResult.CumulativeReturns.ToArray());
        formsPlot1.Plot.Axes.DateTimeTicksBottom();
        formsPlot1.Refresh();

        sharpeRatioValue.Text = runResult.AnnualizedSharpeRatio.ToString();
        maxDrawdownValue.Text = runResult.MaximumDrawdown.ToString();
        maxDrawdownDurationValue.Text = runResult.MaximumDrawdownDuration.ToString();
    }

    private void ScatterPlotForm_Load(object sender, EventArgs e)
    {

    }

    private void label1_Click(object sender, EventArgs e)
    {

    }

    private void textBox1_TextChanged(object sender, EventArgs e)
    {

    }
}
