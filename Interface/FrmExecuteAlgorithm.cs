using System;
using System.ComponentModel;
using System.Windows.Forms;
using BusinessRules.ExtractiveSummarizer;
using BusinessRules.ExtractiveSummarizer.Graphs;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteFSP;
using BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteSFLA;
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
                        OptimizacionProbability = 0.4,
                        MaxNumberOfOptimizacions = 5,
                        Alfa = 0.55,
                        Beta = 0.15,
                        Gamma = 0.10,
                        Delta = 0.10,
                        Ro = 0.10
                    };
                    break;
                case "FSP":
                    mySummaryParameters = new FSPParameters
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
                        TheFitnessFunction = FitnessFunction.MASDS,
                        T = 7,
                        N = 13,
                        L = 1,
                        M = 20,
                        C = 1,
                        Alfa = 0.55,
                        Beta = 0.15,
                        Gamma = 0.10,
                        Delta = 0.10,
                        Ro = 0.10
                    };
                    break;
                case "SFLA":
                    mySummaryParameters = new SFLAParameters
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
                        TheFitnessFunction = FitnessFunction.MASDS,
                        M = 20,
                        C = 1,
                        Tenure =8,
                        PondSize = 20,
                        NumberOfMemeplexes = 5,
                        MaxLocalIterations = 10,
                        ProbabilityOfMutation = 0.06,
                        Alfa = 0.55,
                        Beta = 0.15,
                        Gamma = 0.10,
                        Delta = 0.10,
                        Ro = 0.10,
                    };
                    break;
            }
            var generator = new GenerarResumenes();
            generator.Ejecutar(_chosenDUC, mySummaryParameters, _experimentId, _totalRepetitions, _algorithm);
            progress += 1;
            backgroundWorker1.ReportProgress(progress/2);
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