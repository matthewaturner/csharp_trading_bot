using ScottPlot.WinForms;

namespace FormRunner
{
    partial class ScatterPlotForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            formsPlot1 = new FormsPlot();
            sharpeRatioValue = new TextBox();
            label1 = new Label();
            label2 = new Label();
            maxDrawdownValue = new TextBox();
            maxDrawdownDurationValue = new TextBox();
            label3 = new Label();
            SuspendLayout();
            // 
            // formsPlot1
            // 
            formsPlot1.DisplayScale = 1F;
            formsPlot1.Location = new Point(14, 13);
            formsPlot1.Name = "formsPlot1";
            formsPlot1.Size = new Size(774, 425);
            formsPlot1.TabIndex = 0;
            // 
            // sharpeRatioValue
            // 
            sharpeRatioValue.Enabled = false;
            sharpeRatioValue.Location = new Point(794, 49);
            sharpeRatioValue.Name = "sharpeRatioValue";
            sharpeRatioValue.Size = new Size(150, 23);
            sharpeRatioValue.TabIndex = 1;
            sharpeRatioValue.TextChanged += textBox1_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(794, 31);
            label1.Name = "label1";
            label1.Size = new Size(70, 15);
            label1.TabIndex = 2;
            label1.Text = "SharpeRatio";
            label1.Click += label1_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(794, 75);
            label2.Name = "label2";
            label2.Size = new Size(89, 15);
            label2.TabIndex = 3;
            label2.Text = "Max Drawdown";
            // 
            // maxDrawdownValue
            // 
            maxDrawdownValue.Enabled = false;
            maxDrawdownValue.Location = new Point(794, 93);
            maxDrawdownValue.Name = "maxDrawdownValue";
            maxDrawdownValue.Size = new Size(150, 23);
            maxDrawdownValue.TabIndex = 4;
            // 
            // maxDrawdownDurationValue
            // 
            maxDrawdownDurationValue.Enabled = false;
            maxDrawdownDurationValue.Location = new Point(794, 137);
            maxDrawdownDurationValue.Name = "maxDrawdownDurationValue";
            maxDrawdownDurationValue.Size = new Size(150, 23);
            maxDrawdownDurationValue.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(794, 119);
            label3.Name = "label3";
            label3.Size = new Size(138, 15);
            label3.TabIndex = 6;
            label3.Text = "Max Drawdown Duration";
            // 
            // ScatterPlotForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(956, 449);
            Controls.Add(label3);
            Controls.Add(maxDrawdownDurationValue);
            Controls.Add(maxDrawdownValue);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(sharpeRatioValue);
            Controls.Add(formsPlot1);
            Name = "ScatterPlotForm";
            Text = "Form1";
            Load += ScatterPlotForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ScottPlot.WinForms.FormsPlot formsPlot1;
        private TextBox sharpeRatioValue;
        private Label label1;
        private Label label2;
        private TextBox maxDrawdownValue;
        private TextBox maxDrawdownDurationValue;
        private Label label3;
    }
}
