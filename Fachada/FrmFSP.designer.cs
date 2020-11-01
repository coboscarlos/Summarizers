namespace Interface
{
    partial class FrmFSP
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
            this.SuspendLayout();
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(13, 203);
            this.txtOutput.Margin = new System.Windows.Forms.Padding(4);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(666, 187);
            this.txtOutput.TabIndex = 1;
            // 
            // btn30DUC2001excel
            // 
            this.btn30DUC2001excel.Location = new System.Drawing.Point(13, 161);
            this.btn30DUC2001excel.Margin = new System.Windows.Forms.Padding(4);
            this.btn30DUC2001excel.Name = "btn30DUC2001excel";
            this.btn30DUC2001excel.Size = new System.Drawing.Size(256, 28);
            this.btn30DUC2001excel.TabIndex = 3;
            this.btn30DUC2001excel.Text = "Prueba sobre el dataset escogido";
            this.btn30DUC2001excel.UseVisualStyleBackColor = true;
            this.btn30DUC2001excel.Click += new System.EventHandler(this.Btn30Duc2005ExcelClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(492, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(195, 17);
            this.label3.TabIndex = 13;
            this.label3.Text = "Ejecuciones por Experimento:";
            // 
            // lsbTotalEjecuciones
            // 
            this.lsbTotalEjecuciones.FormattingEnabled = true;
            this.lsbTotalEjecuciones.ItemHeight = 16;
            this.lsbTotalEjecuciones.Items.AddRange(new object[] {
            "1",
            "3",
            "5",
            "30"});
            this.lsbTotalEjecuciones.Location = new System.Drawing.Point(492, 32);
            this.lsbTotalEjecuciones.Name = "lsbTotalEjecuciones";
            this.lsbTotalEjecuciones.Size = new System.Drawing.Size(187, 116);
            this.lsbTotalEjecuciones.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Id Experimento:";
            // 
            // lsbIdExperimento
            // 
            this.lsbIdExperimento.FormattingEnabled = true;
            this.lsbIdExperimento.ItemHeight = 16;
            this.lsbIdExperimento.Items.AddRange(new object[] {
            "1",
            "10000",
            "20000",
            "30000",
            "40000",
            "50000",
            "60000"});
            this.lsbIdExperimento.Location = new System.Drawing.Point(271, 32);
            this.lsbIdExperimento.Name = "lsbIdExperimento";
            this.lsbIdExperimento.Size = new System.Drawing.Size(187, 116);
            this.lsbIdExperimento.TabIndex = 10;
            // 
            // lsbDataSet
            // 
            this.lsbDataSet.FormattingEnabled = true;
            this.lsbDataSet.ItemHeight = 16;
            this.lsbDataSet.Items.AddRange(new object[] {
            "DUC2001",
            "DUC2002"});
            this.lsbDataSet.Location = new System.Drawing.Point(13, 32);
            this.lsbDataSet.Name = "lsbDataSet";
            this.lsbDataSet.Size = new System.Drawing.Size(187, 116);
            this.lsbDataSet.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 17);
            this.label1.TabIndex = 15;
            this.label1.Text = "dataSet";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(295, 166);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(384, 23);
            this.progressBar1.TabIndex = 16;
            // 
            // FrmFSP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 403);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lsbDataSet);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lsbTotalEjecuciones);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lsbIdExperimento);
            this.Controls.Add(this.btn30DUC2001excel);
            this.Controls.Add(this.txtOutput);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmFSP";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FSP";
            this.Load += new System.EventHandler(this.FrmFSPDiscreto_Load);
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
    }
}