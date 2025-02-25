namespace FormRunner
{
    public partial class BacktestResultForm : Form
    {
        public BacktestResultForm()
        {
            InitializeComponent();

            // Example data: X and Y values
            double[] dataX = { 1, 2, 3, 4, 5 };
            double[] dataY = { 1, 4, 9, 16, 25 };

            // Plotting: add a scatter plot
            formsPlot1.Plot.Add.Scatter(dataX, dataY);

            formsPlot1.Refresh();
        }
    }
}
