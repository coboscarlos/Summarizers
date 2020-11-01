using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using BusinessRules.ExtractiveSummarizer.Graphs;
using BusinessRules.VectorSpaceModel;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS;
using BusinessRules.Utils;

namespace Interface
{
    public partial class FrmExecuteAlgorithm : Form
    {
        public FrmExecuteAlgorithm()
        {
            InitializeComponent();
        }

        private void FrmGBHS_Load(object sender, EventArgs e)
        {
            lsbModelos.Text = @"BM25";
            lsAlgorithm.Text = @"LexRankWithThreshold";
            lsbDataSet.Text = @"CnnDm";
            lsbDocRep.Text = @"Vector";
            lsbIdExperimento.Text = @"10000";
            lsbTotalEjecuciones.Text = @"1";
            lstNormalized.Text = @"true";
        }

        private DUCDataSet _chosenDUC;
        private int _totalRepetitions;
        private int _experimentId;
        private TFIDFWeight  _weight;
        private DocumentRepresentation _docRep;
        private string _algorithm;
        private bool _normalized;

        private void Btn30Duc2005ExcelClick(object sender, EventArgs e)
        {
            var d1 = new DUCDataSet("DUC2001",
                @"D:\off-line\mineria-de-datos\datos\Evaluacion-ROUGE\DUC2001\evaluacion\",
                @"D:\off-line\mineria-de-datos\datos\Matrices\DUC2001\");
            var d2 = new DUCDataSet("DUC2002",
                @"D:\off-line\mineria-de-datos\datos\Evaluacion-ROUGE\DUC2002\evaluacion\",
                @"D:\off-line\mineria-de-datos\datos\Matrices\DUC2002\");
            var d3 = new DUCDataSet("CnnDm",
                @"D:\off-line\mineria-de-datos\datos\Evaluacion-ROUGE\CnnDm\evaluacion\",
                @"D:\off-line\mineria-de-datos\datos\Matrices\CnnDm\");
            switch (lsbDataSet.SelectedItem.ToString())
            {
                case "DUC2001": _chosenDUC = d1;
                    break;
                case "DUC2002": _chosenDUC = d2;
                    break;
                case "CnnDm": _chosenDUC = d3;
                    break;
            }

            _totalRepetitions = int.Parse((string)lsbTotalEjecuciones.SelectedItem);
            _experimentId = int.Parse((string)lsbIdExperimento.SelectedItem);
            _weight = (TFIDFWeight)Enum.Parse(typeof(TFIDFWeight ), (string)lsbModelos.SelectedItem);
            _docRep = (DocumentRepresentation)Enum.Parse(typeof(DocumentRepresentation), (string)lsbDocRep.SelectedItem);
            _normalized = bool.Parse((string)lstNormalized.SelectedItem);
            _algorithm = (string) lsAlgorithm.SelectedItem;

            btn30DUC2001excel.Enabled = false;
            backgroundWorker1.RunWorkerAsync();
        }

        private void BackgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var progress = 0;
            backgroundWorker1.ReportProgress(0);

            var list = new List<TestValues>();
            //var b00 = new TestValues(0.50, 0.00, 0.30, 0.00, 0.20); lista.Add(b00);
            //var b01 = new TestValues(0.50, 0.00, 0.32, 0.00, 0.18); lista.Add(b01);
            //var b02 = new TestValues(0.50, 0.00, 0.28, 0.00, 0.22); lista.Add(b02);
            //var b03 = new TestValues(0.48, 0.00, 0.30, 0.00, 0.22); lista.Add(b03);
            //var b04 = new TestValues(0.48, 0.00, 0.32, 0.00, 0.20); lista.Add(b04);
            //var b05 = new TestValues(0.52, 0.00, 0.30, 0.00, 0.18); lista.Add(b05);
            var b06 = new TestValues(0.52, 0.00, 0.28, 0.00, 0.20); list.Add(b06);
            //var b07 = new TestValues(0.40, 0.00, 0.30, 0.00, 0.30); lista.Add(b07);
            //var b08 = new TestValues(0.30, 0.00, 0.40, 0.00, 0.30); lista.Add(b08);
            //var b09 = new TestValues(0.30, 0.00, 0.30, 0.00, 0.40); lista.Add(b09);
            //var b10 = new TestValues(0.20, 0.00, 0.40, 0.00, 0.40); lista.Add(b10);
            //for (var alfa=0.48; alfa <=0.53; alfa+=0.01)
            //    for (var gamma = 0.28; gamma <= 0.33; gamma += 0.01)
            //        for (var ro = 0.18; ro <= 0.23; ro += 0.01)
            //        {
            //            if (Math.Abs(alfa + gamma + ro - 1.0) < 0.00001)
            //            {
            //                var nuevo = new TestValues(alfa, 0.00, gamma, 0.00, ro);
            //                lista.Add(nuevo);
            //            }
            //        }

            //for (var op = 0.1; op <= 1.0; op += 0.1)
            //{
            //    for (var numopt = 5; numopt < 20; numopt += 5)
            //    {
                    foreach (var par in list)
                    {
                        SummaryParameters mySummaryParameters = null;
                        switch (_algorithm)
                        {
                            case "LexRankWithThreshold":
                                mySummaryParameters = new LexRankWithThresholdParameters
                                {
                                    DetailedReport = false,
                                    MySummaryType = SummaryType.Words,
                                    MaximumLengthOfSummaryForRouge = 100,
                                    MyTDMParameters = new TDMParameters
                                    {
                                        MinimumFrequencyThresholdOfTermsForPhrase = 0,
                                        MinimumThresholdForTheAcceptanceOfThePhrase = 0.0,
                                        TheDocumentRepresentation = _docRep,
                                        TheTFIDFWeight = _weight
                                    },
                                    Threshold = 0.1,
                                    DampingFactor = 0.15,
                                    ErrorTolerance = 0.1,
                                    SimilarityNormalized = _normalized
                                };
                                break;
                            case "ContinuousLexRank":
                                        mySummaryParameters = new ContinuousLexRankParameters
                                        {
                                            DetailedReport = false,
                                            MySummaryType = SummaryType.Words,
                                            MaximumLengthOfSummaryForRouge = 100,
                                            MyTDMParameters = new TDMParameters
                                            {
                                                MinimumFrequencyThresholdOfTermsForPhrase = 0,
                                                MinimumThresholdForTheAcceptanceOfThePhrase = 0.0,
                                                TheDocumentRepresentation = _docRep,
                                                TheTFIDFWeight = _weight
                                            },
                                            DampingFactor = 0.15,
                                            ErrorTolerance = 0.1,
                                            SimilarityNormalized = _normalized
                                        };
                                        break;

                            case "GBHS":
                                        mySummaryParameters = new GBHSParameters
                                        {
                                            DetailedReport = false,
                                            MySummaryType = SummaryType.Words,
                                            MaximumLengthOfSummaryForRouge = 100,
                                            MaximumSummaryLengthToEvolve = 110,
                                            MyTDMParameters = new TDMParameters
                                            {
                                                MinimumFrequencyThresholdOfTermsForPhrase = 0, // 0 y 0 son los dos valores originales
                                                MinimumThresholdForTheAcceptanceOfThePhrase = 0.0,
                                                TheDocumentRepresentation = _docRep,
                                                TheTFIDFWeight = _weight
                                            },
                                            MaximumNumberOfFitnessFunctionEvaluations = 1600,
                                            HMS = 10,
                                            HMCR = 0.95,
                                            ParMin = 0.01,
                                            ParMax = 0.99,
                                            TheFitnessFunction = FitnessFunction.MASDS,
                                            Alfa = par.Alpha,
                                            Beta = par.Beta,
                                            Gamma = par.Gamma,
                                            Delta = par.Delta,
                                            Ro = par.Ro,
                                            ProbabilidadOptimizacion = 0.4,
                                            MaximoNumeroOptimizacion = 5
                                        };
                                        break;
                        }

                        var generator = new GenerarResumenes();
                        generator.Ejecutar(_chosenDUC, mySummaryParameters, _experimentId, _totalRepetitions, _algorithm);
                        progress += 1;
                        backgroundWorker1.ReportProgress(progress/2);
                    }
            //    }
            //}

            //var misParametrosDePruebaGBHS = ParametrosDePruebaGBHS.Instance;
            //var misParametros = misParametrosDePruebaGBHS.GetLast();
            //while (misParametros != null)
            //{
            //    var generador = new GenerarResumenes();
            //    generador.Ejecutar(_chosenDUC, misParametros, _experimentId, _totalRepetitions, "GBHS");
            //    progreso += 1;
            //    backgroundWorker1.ReportProgress(progreso/2);
            //    misParametros = misParametrosDePruebaGBHS.GetLast();
            //}
        }

        private void BackgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void BackgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btn30DUC2001excel.Enabled = true;
            txtOutput.Text += @"All experiments finished";
        }
    }
}