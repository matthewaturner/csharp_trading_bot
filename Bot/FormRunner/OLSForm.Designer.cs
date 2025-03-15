namespace FormRunner;

partial class OLSForm
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
        label1 = new Label();
        slopeValue = new TextBox();
        label2 = new Label();
        intersectValue = new TextBox();
        SuspendLayout();
        // 
        // formsPlot1
        // 
        formsPlot1.DisplayScale = 1F;
        formsPlot1.Location = new Point(12, 12);
        formsPlot1.Name = "formsPlot1";
        formsPlot1.Size = new Size(775, 426);
        formsPlot1.TabIndex = 0;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(44, 441);
        label1.Name = "label1";
        label1.Size = new Size(36, 15);
        label1.TabIndex = 1;
        label1.Text = "Slope";
        label1.Click += label1_Click;
        // 
        // slopeValue
        // 
        slopeValue.Location = new Point(86, 438);
        slopeValue.Name = "slopeValue";
        slopeValue.Size = new Size(147, 23);
        slopeValue.TabIndex = 2;
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(250, 441);
        label2.Name = "label2";
        label2.Size = new Size(52, 15);
        label2.TabIndex = 3;
        label2.Text = "Intersect";
        // 
        // intersectValue
        // 
        intersectValue.Location = new Point(308, 438);
        intersectValue.Name = "intersectValue";
        intersectValue.Size = new Size(155, 23);
        intersectValue.TabIndex = 4;
        // 
        // OLSForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(799, 470);
        Controls.Add(intersectValue);
        Controls.Add(label2);
        Controls.Add(slopeValue);
        Controls.Add(label1);
        Controls.Add(formsPlot1);
        Name = "OLSForm";
        Text = "OLSForm";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private ScottPlot.WinForms.FormsPlot formsPlot1;
    private Label label1;
    private TextBox slopeValue;
    private Label label2;
    private TextBox intersectValue;
}