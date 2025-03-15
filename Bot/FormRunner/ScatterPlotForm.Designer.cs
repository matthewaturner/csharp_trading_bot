namespace FormRunner;

partial class ScatterPlotForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        formsPlot1 = new ScottPlot.WinForms.FormsPlot();
        richTextBox1 = new RichTextBox();
        SuspendLayout();
        // 
        // formsPlot1
        // 
        formsPlot1.DisplayScale = 1F;
        formsPlot1.Location = new Point(12, 12);
        formsPlot1.Name = "formsPlot1";
        formsPlot1.Size = new Size(812, 465);
        formsPlot1.TabIndex = 0;
        // 
        // richTextBox1
        // 
        richTextBox1.BackColor = SystemColors.ControlLightLight;
        richTextBox1.Location = new Point(830, 26);
        richTextBox1.Name = "richTextBox1";
        richTextBox1.ReadOnly = true;
        richTextBox1.Size = new Size(221, 451);
        richTextBox1.TabIndex = 1;
        richTextBox1.Text = "";
        // 
        // ScatterPlotForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1063, 489);
        Controls.Add(richTextBox1);
        Controls.Add(formsPlot1);
        Name = "ScatterPlotForm";
        Text = "ScatterPlotForm";
        ResumeLayout(false);
    }

    #endregion

    private ScottPlot.WinForms.FormsPlot formsPlot1;
    private RichTextBox richTextBox1;
}