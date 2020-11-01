namespace Fachada
{
    partial class SumarizacionHS
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtAlfa = new System.Windows.Forms.TextBox();
            this.txtGamma = new System.Windows.Forms.TextBox();
            this.txtBeta = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.mostrarurls = new System.Windows.Forms.Button();
            this.urlCargada = new System.Windows.Forms.TextBox();
            this.cargar = new System.Windows.Forms.Button();
            this.btoExaminar = new System.Windows.Forms.Button();
            this.txtExaminar = new System.Windows.Forms.TextBox();
            this.btoExaminarPR = new System.Windows.Forms.Button();
            this.txtExaminarPR = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btoResumenModelo = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtResultados = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtHMCR = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbxNi = new System.Windows.Forms.ComboBox();
            this.btoResumen = new System.Windows.Forms.Button();
            this.cbxCantidadPalabras = new System.Windows.Forms.ComboBox();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.txtResumenModelo = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtAlfa
            // 
            this.txtAlfa.Location = new System.Drawing.Point(71, 22);
            this.txtAlfa.Name = "txtAlfa";
            this.txtAlfa.Size = new System.Drawing.Size(33, 20);
            this.txtAlfa.TabIndex = 1;
            this.txtAlfa.Text = "1";
            // 
            // txtGamma
            // 
            this.txtGamma.Location = new System.Drawing.Point(302, 18);
            this.txtGamma.Name = "txtGamma";
            this.txtGamma.Size = new System.Drawing.Size(40, 20);
            this.txtGamma.TabIndex = 2;
            this.txtGamma.Text = "1";
            // 
            // txtBeta
            // 
            this.txtBeta.Location = new System.Drawing.Point(173, 22);
            this.txtBeta.Name = "txtBeta";
            this.txtBeta.Size = new System.Drawing.Size(37, 20);
            this.txtBeta.TabIndex = 3;
            this.txtBeta.Text = "1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cobertura: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(110, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Cohesion: ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(231, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Redundancia:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtAlfa);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtGamma);
            this.groupBox1.Controls.Add(this.txtBeta);
            this.groupBox1.Location = new System.Drawing.Point(12, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(348, 51);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Coeficientes";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.mostrarurls);
            this.groupBox2.Controls.Add(this.urlCargada);
            this.groupBox2.Controls.Add(this.cargar);
            this.groupBox2.Controls.Add(this.btoExaminar);
            this.groupBox2.Controls.Add(this.txtExaminar);
            this.groupBox2.Location = new System.Drawing.Point(12, 96);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(348, 115);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Archivo";
            // 
            // mostrarurls
            // 
            this.mostrarurls.Location = new System.Drawing.Point(247, 86);
            this.mostrarurls.Name = "mostrarurls";
            this.mostrarurls.Size = new System.Drawing.Size(75, 23);
            this.mostrarurls.TabIndex = 4;
            this.mostrarurls.Text = "Total";
            this.mostrarurls.UseVisualStyleBackColor = true;
            this.mostrarurls.Click += new System.EventHandler(this.mostrarurls_Click);
            // 
            // urlCargada
            // 
            this.urlCargada.Location = new System.Drawing.Point(167, 19);
            this.urlCargada.Multiline = true;
            this.urlCargada.Name = "urlCargada";
            this.urlCargada.Size = new System.Drawing.Size(155, 61);
            this.urlCargada.TabIndex = 3;
            // 
            // cargar
            // 
            this.cargar.Location = new System.Drawing.Point(99, 57);
            this.cargar.Name = "cargar";
            this.cargar.Size = new System.Drawing.Size(62, 23);
            this.cargar.TabIndex = 2;
            this.cargar.Text = "Cargar";
            this.cargar.UseVisualStyleBackColor = true;
            this.cargar.Click += new System.EventHandler(this.cargar_Click);
            // 
            // btoExaminar
            // 
            this.btoExaminar.Location = new System.Drawing.Point(99, 28);
            this.btoExaminar.Name = "btoExaminar";
            this.btoExaminar.Size = new System.Drawing.Size(62, 23);
            this.btoExaminar.TabIndex = 1;
            this.btoExaminar.Text = "Examinar";
            this.btoExaminar.UseVisualStyleBackColor = true;
            this.btoExaminar.Click += new System.EventHandler(this.btoExaminar_Click);
            // 
            // txtExaminar
            // 
            this.txtExaminar.Location = new System.Drawing.Point(6, 30);
            this.txtExaminar.Name = "txtExaminar";
            this.txtExaminar.Size = new System.Drawing.Size(86, 20);
            this.txtExaminar.TabIndex = 0;
            this.txtExaminar.Text = "C:\\Users\\W!!!\\Desktop\\Documentos de prueba\\AP880911-0016";
            // 
            // btoExaminarPR
            // 
            this.btoExaminarPR.Location = new System.Drawing.Point(155, 33);
            this.btoExaminarPR.Name = "btoExaminarPR";
            this.btoExaminarPR.Size = new System.Drawing.Size(75, 23);
            this.btoExaminarPR.TabIndex = 2;
            this.btoExaminarPR.Text = "Examinar";
            this.btoExaminarPR.UseVisualStyleBackColor = true;
            this.btoExaminarPR.Click += new System.EventHandler(this.btoExaminarPR_Click);
            // 
            // txtExaminarPR
            // 
            this.txtExaminarPR.Location = new System.Drawing.Point(6, 33);
            this.txtExaminarPR.Name = "txtExaminarPR";
            this.txtExaminarPR.Size = new System.Drawing.Size(122, 20);
            this.txtExaminarPR.TabIndex = 1;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btoResumenModelo);
            this.groupBox3.Controls.Add(this.btoExaminarPR);
            this.groupBox3.Controls.Add(this.txtExaminarPR);
            this.groupBox3.Location = new System.Drawing.Point(83, 217);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(277, 103);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Resumen Modelo";
            // 
            // btoResumenModelo
            // 
            this.btoResumenModelo.Location = new System.Drawing.Point(155, 62);
            this.btoResumenModelo.Name = "btoResumenModelo";
            this.btoResumenModelo.Size = new System.Drawing.Size(75, 23);
            this.btoResumenModelo.TabIndex = 3;
            this.btoResumenModelo.Text = "Obtener";
            this.btoResumenModelo.UseVisualStyleBackColor = true;
            this.btoResumenModelo.Click += new System.EventHandler(this.btoResumenModelo_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(844, 24);
            this.menuStrip1.TabIndex = 10;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.salirToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(60, 20);
            this.toolStripMenuItem1.Text = "Archivo";
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            this.salirToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.salirToolStripMenuItem.Text = "Salir";
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);
            // 
            // txtResultados
            // 
            this.txtResultados.Location = new System.Drawing.Point(22, 29);
            this.txtResultados.Multiline = true;
            this.txtResultados.Name = "txtResultados";
            this.txtResultados.Size = new System.Drawing.Size(389, 256);
            this.txtResultados.TabIndex = 11;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtResultados);
            this.groupBox4.Location = new System.Drawing.Point(388, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(434, 307);
            this.groupBox4.TabIndex = 12;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Resumen";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtHMCR);
            this.groupBox5.Controls.Add(this.label7);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.cbxNi);
            this.groupBox5.Controls.Add(this.btoResumen);
            this.groupBox5.Controls.Add(this.cbxCantidadPalabras);
            this.groupBox5.Location = new System.Drawing.Point(45, 326);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(315, 212);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Crear resumen";
            // 
            // txtHMCR
            // 
            this.txtHMCR.Location = new System.Drawing.Point(179, 104);
            this.txtHMCR.Name = "txtHMCR";
            this.txtHMCR.Size = new System.Drawing.Size(72, 20);
            this.txtHMCR.TabIndex = 9;
            this.txtHMCR.Text = "0,34";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 104);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Valor de HMCR:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Cantidad Palabras Resumen";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 69);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(146, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Maxima Cantidad Iteraciones:";
            // 
            // cbxNi
            // 
            this.cbxNi.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxNi.FormattingEnabled = true;
            this.cbxNi.Items.AddRange(new object[] {
            "1",
            "5",
            "20",
            "50",
            "100",
            "200",
            "500",
            "1000",
            "1500",
            "2000",
            "5000"});
            this.cbxNi.Location = new System.Drawing.Point(176, 61);
            this.cbxNi.Name = "cbxNi";
            this.cbxNi.Size = new System.Drawing.Size(72, 21);
            this.cbxNi.TabIndex = 3;
            // 
            // btoResumen
            // 
            this.btoResumen.Location = new System.Drawing.Point(199, 157);
            this.btoResumen.Name = "btoResumen";
            this.btoResumen.Size = new System.Drawing.Size(90, 35);
            this.btoResumen.TabIndex = 2;
            this.btoResumen.Text = "Resumir";
            this.btoResumen.UseVisualStyleBackColor = true;
            this.btoResumen.Click += new System.EventHandler(this.btoResumen_Click);
            // 
            // cbxCantidadPalabras
            // 
            this.cbxCantidadPalabras.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxCantidadPalabras.FormattingEnabled = true;
            this.cbxCantidadPalabras.Items.AddRange(new object[] {
            "50",
            "100",
            "200",
            "300",
            "400"});
            this.cbxCantidadPalabras.Location = new System.Drawing.Point(176, 25);
            this.cbxCantidadPalabras.Name = "cbxCantidadPalabras";
            this.cbxCantidadPalabras.Size = new System.Drawing.Size(72, 21);
            this.cbxCantidadPalabras.TabIndex = 1;
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.FileName = "openFileDialog2";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.txtResumenModelo);
            this.groupBox7.Location = new System.Drawing.Point(388, 338);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(428, 200);
            this.groupBox7.TabIndex = 3;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Resumen Modelo";
            // 
            // txtResumenModelo
            // 
            this.txtResumenModelo.AcceptsReturn = true;
            this.txtResumenModelo.AcceptsTab = true;
            this.txtResumenModelo.Location = new System.Drawing.Point(22, 28);
            this.txtResumenModelo.Multiline = true;
            this.txtResumenModelo.Name = "txtResumenModelo";
            this.txtResumenModelo.Size = new System.Drawing.Size(389, 152);
            this.txtResumenModelo.TabIndex = 0;
            // 
            // SumarizacionHS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 572);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "SumarizacionHS";
            this.Text = "SumarizacionHS";
            this.Load += new System.EventHandler(this.SumarizacionHS_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAlfa;
        private System.Windows.Forms.TextBox txtGamma;
        private System.Windows.Forms.TextBox txtBeta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.Button btoExaminar;
        private System.Windows.Forms.TextBox txtExaminar;
        private System.Windows.Forms.TextBox txtResultados;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cbxCantidadPalabras;
        private System.Windows.Forms.TextBox txtExaminarPR;
        private System.Windows.Forms.Button btoExaminarPR;
        private System.Windows.Forms.OpenFileDialog openFileDialog2;
        private System.Windows.Forms.Button btoResumen;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.TextBox txtResumenModelo;
        private System.Windows.Forms.Button btoResumenModelo;
        private System.Windows.Forms.ComboBox cbxNi;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtHMCR;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox urlCargada;
        private System.Windows.Forms.Button cargar;
        private System.Windows.Forms.Button mostrarurls;
    }
}

