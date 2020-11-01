using System;
using System.ComponentModel;
using System.Windows.Forms;
using BusinessRules.Utils;

namespace Interface
{
    public partial class FrmFSP : Form
    {
        public FrmFSP()
        {
            InitializeComponent();
        }

        private void FrmFSPDiscreto_Load(object sender, EventArgs e)
        {
            lsbIdExperimento.Text = @"1";
            lsbTotalEjecuciones.Text = @"5";
        }

        private DUCDataSet _dsElegiido;
        private int _totalEjecuciones;
        private int _idExperimento;

        private void Btn30Duc2005ExcelClick(object sender, EventArgs e)
        {
            var d1 = new DUCDataSet("DUC2001",
                @"D:\off-line\mineria-de-datos\datos\Evaluacion-ROUGE\DUC2001\evaluacion\",
                @"D:\off-line\mineria-de-datos\datos\Matrices\DUC2001\");
            var d2 = new DUCDataSet("DUC2002",
                @"D:\off-line\mineria-de-datos\datos\Evaluacion-ROUGE\DUC2002\evaluacion\",
                @"D:\off-line\mineria-de-datos\datos\Matrices\DUC2002\");

            switch (lsbDataSet.SelectedItem.ToString())
            {
                case "DUC2001": _dsElegiido = d1;
                    break;
                case "DUC2002": _dsElegiido = d2;
                    break;
            }

            _totalEjecuciones = int.Parse((string)lsbTotalEjecuciones.SelectedItem);
            _idExperimento = int.Parse((string)lsbIdExperimento.SelectedItem);

            backgroundWorker1.RunWorkerAsync();
            btn30DUC2001excel.Enabled = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //Se asigna el valor a los parametros, teniendo en cuenta los parametros que se definieron 
            //en la forma o los que estan por defecto en la clase Parametros
            var progreso = 0;
            backgroundWorker1.ReportProgress(0);
            var misParametrosDePruebaGBHS = ParametrosDePruebaFSP.Instance;
            var misParametros = misParametrosDePruebaGBHS.GetLast();
            while (misParametros != null)
            {
                var generador = new GenerarResumenes();
                generador.Ejecutar(_dsElegiido, misParametros, _idExperimento, _totalEjecuciones, "DiscreteFSP");
                progreso += 1;
                backgroundWorker1.ReportProgress(progreso / 2);
                misParametros = misParametrosDePruebaGBHS.GetLast();
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            if (e.ProgressPercentage == 100)
                txtOutput.Text += @"LISTO " + (string)e.UserState + @"\r\n";
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            txtOutput.Text = @"Todos los Experimentos Completados";
        }
    }
}