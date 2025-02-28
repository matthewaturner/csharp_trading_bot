namespace FormRunner
{
    public partial class ScatterPlotForm : Form
    {
        public ScatterPlotForm(DateTime[] xData, decimal[] yData)
        {
            InitializeComponent();
            formsPlot1.Plot.Add.Scatter(xData.Select(d => d.ToOADate()).ToArray(), yData);
            formsPlot1.Plot.Axes.DateTimeTicksBottom();
            formsPlot1.Refresh();
        }
    }
}
