namespace Interface
{
    partial class FrmExecuteAlgorithm
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
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btn30DUC2001excel = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lsbTotalEjecuciones = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lsbIdExperimento = new System.Windows.Forms.ListBox();
            this.lsbDataSet = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.lsbModelos = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lsbDocRep = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lsAlgorithm = new System.Windows.Forms.ListBox();
            this.label7 = new System.Windows.Forms.Label();
            this.lstNormalized = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(10, 165);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(627, 153);
            this.txtOutput.TabIndex = 1;
            // 
            // btn30DUC2001excel
            // 
            this.btn30DUC2001excel.Location = new System.Drawing.Point(10, 131);
            this.btn30DUC2001excel.Name = "btn30DUC2001excel";
            this.btn30DUC2001excel.Size = new System.Drawing.Size(192, 23);
            this.btn30DUC2001excel.TabIndex = 3;
            this.btn30DUC2001excel.Text = "Test over selected dataset";
            this.btn30DUC2001excel.UseVisualStyleBackColor = true;
            this.btn30DUC2001excel.Click += new System.EventHandler(this.Btn30Duc2005ExcelClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(294, 10);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Repetitions:";
            // 
            // lsbTotalEjecuciones
            // 
            this.lsbTotalEjecuciones.FormattingEnabled = true;
            this.lsbTotalEjecuciones.Items.AddRange(new object[] {
            "1",
            "3",
            "5",
            "30"});
            this.lsbTotalEjecuciones.Location = new System.Drawing.Point(294, 26);
            this.lsbTotalEjecuciones.Margin = new System.Windows.Forms.Padding(2);
            this.lsbTotalEjecuciones.Name = "lsbTotalEjecuciones";
            this.lsbTotalEjecuciones.Size = new System.Drawing.Size(72, 95);
            this.lsbTotalEjecuciones.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(211, 10);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Id Experiment:";
            // 
            // lsbIdExperimento
            // 
            this.lsbIdExperimento.FormattingEnabled = true;
            this.lsbIdExperimento.Items.AddRange(new object[] {
            "10000",
            "20000",
            "30000",
            "40000",
            "50000",
            "60000",
            "70000",
            "80000",
            "90000"});
            this.lsbIdExperimento.Location = new System.Drawing.Point(211, 26);
            this.lsbIdExperimento.Margin = new System.Windows.Forms.Padding(2);
            this.lsbIdExperimento.Name = "lsbIdExperimento";
            this.lsbIdExperimento.Size = new System.Drawing.Size(80, 95);
            this.lsbIdExperimento.TabIndex = 10;
            // 
            // lsbDataSet
            // 
            this.lsbDataSet.FormattingEnabled = true;
            this.lsbDataSet.Items.AddRange(new object[] {
            "DUC2001",
            "DUC2002",
            "CnnDm"});
            this.lsbDataSet.Location = new System.Drawing.Point(10, 26);
            this.lsbDataSet.Margin = new System.Windows.Forms.Padding(2);
            this.lsbDataSet.Name = "lsbDataSet";
            this.lsbDataSet.Size = new System.Drawing.Size(58, 95);
            this.lsbDataSet.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 10);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "DataSet";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.BackgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BackgroundWorker1_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(221, 135);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(2);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(288, 19);
            this.progressBar1.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(70, 10);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 18;
            this.label4.Text = "Model Rep:";
            // 
            // lsbModelos
            // 
            this.lsbModelos.FormattingEnabled = true;
            this.lsbModelos.Items.AddRange(new object[] {
            "Simple",
            "Best",
            "Complete",
            "BM25",
            "Doc2Vec"});
            this.lsbModelos.Location = new System.Drawing.Point(71, 26);
            this.lsbModelos.Margin = new System.Windows.Forms.Padding(2);
            this.lsbModelos.Name = "lsbModelos";
            this.lsbModelos.Size = new System.Drawing.Size(66, 95);
            this.lsbModelos.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(140, 10);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 13);
            this.label5.TabIndex = 20;
            this.label5.Text = "Doc Rep:";
            // 
            // lsbDocRep
            // 
            this.lsbDocRep.FormattingEnabled = true;
            this.lsbDocRep.Items.AddRange(new object[] {
            "Vector",
            "Centroid"});
            this.lsbDocRep.Location = new System.Drawing.Point(141, 26);
            this.lsbDocRep.Margin = new System.Windows.Forms.Padding(2);
            this.lsbDocRep.Name = "lsbDocRep";
            this.lsbDocRep.Size = new System.Drawing.Size(66, 95);
            this.lsbDocRep.TabIndex = 19;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(458, 10);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Algorithm";
            // 
            // lsAlgorithm
            // 
            this.lsAlgorithm.FormattingEnabled = true;
            this.lsAlgorithm.Items.AddRange(new object[] {
            "LexRankWithThreshold",
            "ContinuousLexRank",
            "GBHS"});
            this.lsAlgorithm.Location = new System.Drawing.Point(458, 26);
            this.lsAlgorithm.Margin = new System.Windows.Forms.Padding(2);
            this.lsAlgorithm.Name = "lsAlgorithm";
            this.lsAlgorithm.Size = new System.Drawing.Size(102, 95);
            this.lsAlgorithm.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(369, 10);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Normalize:";
            // 
            // lstNormalized
            // 
            this.lstNormalized.FormattingEnabled = true;
            this.lstNormalized.Items.AddRange(new object[] {
            "true",
            "false"});
            this.lstNormalized.Location = new System.Drawing.Point(371, 25);
            this.lstNormalized.Margin = new System.Windows.Forms.Padding(2);
            this.lstNormalized.Name = "lstNormalized";
            this.lstNormalized.Size = new System.Drawing.Size(72, 95);
            this.lstNormalized.TabIndex = 24;
            // 
            // FrmExecuteAlgorithm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(650, 327);
            this.Controls.Add(this.lstNormalized);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lsAlgorithm);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lsbDocRep);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lsbModelos);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lsbDataSet);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lsbTotalEjecuciones);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lsbIdExperimento);
            this.Controls.Add(this.btn30DUC2001excel);
            this.Controls.Add(this.txtOutput);
            this.Name = "FrmExecuteAlgorithm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Execute algorithm";
            this.Load += new System.EventHandler(this.FrmGBHS_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btn30DUC2001excel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox lsbTotalEjecuciones;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lsbIdExperimento;
        private System.Windows.Forms.ListBox lsbDataSet;
        private System.Windows.Forms.Label label1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox lsbModelos;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ListBox lsbDocRep;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox lsAlgorithm;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ListBox lstNormalized;
    }
}