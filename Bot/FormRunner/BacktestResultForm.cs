// -----------------------------------------------------------------------
//     Copyright (c) 2025 Matthew Turner.
//     Licensed under the MIT-NC License (Non-Commercial).
// -----------------------------------------------------------------------

using Bot.Models.Results;
using System.Runtime.CompilerServices;

namespace FormRunner;

public partial class BacktestResultForm : Form
{
    public BacktestResultForm(RunResult runResult, [CallerMemberName] string? name = "Run Result")
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
}
