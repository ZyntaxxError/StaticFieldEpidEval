namespace StaticFieldEpidEval
{
    partial class WeightedGradientForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelDoseDiff = new System.Windows.Forms.Label();
            this.textBoxDoseDiff = new System.Windows.Forms.TextBox();
            this.labelDTA = new System.Windows.Forms.Label();
            this.textBoxDTA = new System.Windows.Forms.TextBox();
            this.buttonPerformAnalysis = new System.Windows.Forms.Button();
            this.labelStatistics = new System.Windows.Forms.Label();
            this.labelGWDDL1 = new System.Windows.Forms.Label();
            this.resultGWDDL1 = new System.Windows.Forms.Label();
            this.labelMaxGWDD = new System.Windows.Forms.Label();
            this.resultMaxGWDD = new System.Windows.Forms.Label();
            this.labelMeanGWDD = new System.Windows.Forms.Label();
            this.resultMeanGWDD = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelDoseDiff
            // 
            this.labelDoseDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDoseDiff.AutoSize = true;
            this.labelDoseDiff.Location = new System.Drawing.Point(12, 573);
            this.labelDoseDiff.Name = "labelDoseDiff";
            this.labelDoseDiff.Size = new System.Drawing.Size(195, 14);
            this.labelDoseDiff.TabIndex = 0;
            this.labelDoseDiff.Text = "Dose Difference (% of max dose):";
            // 
            // textBoxDoseDiff
            // 
            this.textBoxDoseDiff.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxDoseDiff.Location = new System.Drawing.Point(213, 570);
            this.textBoxDoseDiff.Name = "textBoxDoseDiff";
            this.textBoxDoseDiff.Size = new System.Drawing.Size(100, 22);
            this.textBoxDoseDiff.TabIndex = 1;
            // 
            // labelDTA
            // 
            this.labelDTA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelDTA.AutoSize = true;
            this.labelDTA.Location = new System.Drawing.Point(339, 573);
            this.labelDTA.Name = "labelDTA";
            this.labelDTA.Size = new System.Drawing.Size(173, 14);
            this.labelDTA.TabIndex = 2;
            this.labelDTA.Text = "Distance to Agreement (mm):";
            // 
            // textBoxDTA
            // 
            this.textBoxDTA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.textBoxDTA.Location = new System.Drawing.Point(518, 570);
            this.textBoxDTA.Name = "textBoxDTA";
            this.textBoxDTA.Size = new System.Drawing.Size(100, 22);
            this.textBoxDTA.TabIndex = 3;
            // 
            // buttonPerformAnalysis
            // 
            this.buttonPerformAnalysis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPerformAnalysis.Location = new System.Drawing.Point(755, 569);
            this.buttonPerformAnalysis.Name = "buttonPerformAnalysis";
            this.buttonPerformAnalysis.Size = new System.Drawing.Size(166, 23);
            this.buttonPerformAnalysis.TabIndex = 4;
            this.buttonPerformAnalysis.Text = "Perform Analysis";
            this.buttonPerformAnalysis.UseVisualStyleBackColor = true;
            this.buttonPerformAnalysis.Click += new System.EventHandler(this.OnPerformAnalysisClicked);
            // 
            // labelStatistics
            // 
            this.labelStatistics.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelStatistics.Location = new System.Drawing.Point(12, 19);
            this.labelStatistics.Name = "labelStatistics";
            this.labelStatistics.Size = new System.Drawing.Size(147, 22);
            this.labelStatistics.TabIndex = 5;
            this.labelStatistics.Text = "Statistics (inside ROI)";
            // 
            // labelGWDDL1
            // 
            this.labelGWDDL1.AutoSize = true;
            this.labelGWDDL1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGWDDL1.Location = new System.Drawing.Point(12, 55);
            this.labelGWDDL1.Name = "labelGWDDL1";
            this.labelGWDDL1.Size = new System.Drawing.Size(100, 14);
            this.labelGWDDL1.TabIndex = 6;
            this.labelGWDDL1.Text = "Area GWDD < 1:";
            // 
            // resultGWDDL1
            // 
            this.resultGWDDL1.AutoSize = true;
            this.resultGWDDL1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultGWDDL1.Location = new System.Drawing.Point(118, 55);
            this.resultGWDDL1.Name = "resultGWDDL1";
            this.resultGWDDL1.Size = new System.Drawing.Size(0, 14);
            this.resultGWDDL1.TabIndex = 7;
            // 
            // labelMaxGWDD
            // 
            this.labelMaxGWDD.AutoSize = true;
            this.labelMaxGWDD.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMaxGWDD.Location = new System.Drawing.Point(12, 88);
            this.labelMaxGWDD.Name = "labelMaxGWDD";
            this.labelMaxGWDD.Size = new System.Drawing.Size(72, 14);
            this.labelMaxGWDD.TabIndex = 8;
            this.labelMaxGWDD.Text = "Max GWDD:";
            // 
            // resultMaxGWDD
            // 
            this.resultMaxGWDD.AutoSize = true;
            this.resultMaxGWDD.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultMaxGWDD.Location = new System.Drawing.Point(118, 88);
            this.resultMaxGWDD.Name = "resultMaxGWDD";
            this.resultMaxGWDD.Size = new System.Drawing.Size(0, 14);
            this.resultMaxGWDD.TabIndex = 9;
            // 
            // labelMeanGWDD
            // 
            this.labelMeanGWDD.AutoSize = true;
            this.labelMeanGWDD.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelMeanGWDD.Location = new System.Drawing.Point(12, 120);
            this.labelMeanGWDD.Name = "labelMeanGWDD";
            this.labelMeanGWDD.Size = new System.Drawing.Size(80, 14);
            this.labelMeanGWDD.TabIndex = 10;
            this.labelMeanGWDD.Text = "Mean GWDD:";
            // 
            // resultMeanGWDD
            // 
            this.resultMeanGWDD.AutoSize = true;
            this.resultMeanGWDD.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultMeanGWDD.Location = new System.Drawing.Point(118, 120);
            this.resultMeanGWDD.Name = "resultMeanGWDD";
            this.resultMeanGWDD.Size = new System.Drawing.Size(0, 14);
            this.resultMeanGWDD.TabIndex = 11;
            // 
            // WeightedGradientForm
            // 
            this.AcceptButton = this.buttonPerformAnalysis;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 606);
            this.Controls.Add(this.resultMeanGWDD);
            this.Controls.Add(this.labelMeanGWDD);
            this.Controls.Add(this.resultMaxGWDD);
            this.Controls.Add(this.labelMaxGWDD);
            this.Controls.Add(this.resultGWDDL1);
            this.Controls.Add(this.labelGWDDL1);
            this.Controls.Add(this.labelStatistics);
            this.Controls.Add(this.buttonPerformAnalysis);
            this.Controls.Add(this.textBoxDTA);
            this.Controls.Add(this.labelDTA);
            this.Controls.Add(this.textBoxDoseDiff);
            this.Controls.Add(this.labelDoseDiff);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(840, 600);
            this.Name = "WeightedGradientForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Perform gradient weighted dose difference analysis";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelDoseDiff;
        private System.Windows.Forms.TextBox textBoxDoseDiff;
        private System.Windows.Forms.Label labelDTA;
        private System.Windows.Forms.TextBox textBoxDTA;
        private System.Windows.Forms.Button buttonPerformAnalysis;
        private System.Windows.Forms.Label labelStatistics;
        private System.Windows.Forms.Label labelGWDDL1;
        private System.Windows.Forms.Label resultGWDDL1;
        private System.Windows.Forms.Label labelMaxGWDD;
        private System.Windows.Forms.Label resultMaxGWDD;
        private System.Windows.Forms.Label labelMeanGWDD;
        private System.Windows.Forms.Label resultMeanGWDD;
    }
}