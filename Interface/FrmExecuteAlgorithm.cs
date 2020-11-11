using System;
using System.ComponentModel;
using System.Configuration;
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
            lsbDataSet.Text = @"DUC2001";
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
            var appSettings = ConfigurationManager.AppSettings;
            
            var d1 = new DUCDataSet("DUC2001",
                appSettings["DUC2001-dirRouge"],
                appSettings["DUC2001-dirMatrix"]);
            var d2 = new DUCDataSet("DUC2002",
                appSettings["DUC2002-dirRouge"],
                appSettings["DUC2002-dirMatrix"]);

            switch (lsbDataSet.SelectedItem.ToString())
            {
                case "DUC2001": _chosenDUC = d1;
                    break;
                case "DUC2002": _chosenDUC = d2;
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
                        TheFinalOrderOfSummary = FinalOrderOfSummary.Position,
                        OptimizacionProbability = 0.4,
                        MaxNumberOfOptimizacions = 5,
                        Alfa = 0.15,
                        Beta = 0.04,
                        Gamma = 0.09,
                        Delta = 0.07,
                        Ro = 0.65
                    };
                    break;
                case "FSP":
                    mySummaryParameters = new FSPParameters
                    {
                        DetailedReport = false,
                        MySummaryType = SummaryType.Words,
                        MaximumLengthOfSummaryForRouge = 100,
                        MyTDMParameters = new TDMParameters
                        {
                            MinimumFrequencyThresholdOfTermsForPhrase = 0, // 0 y 0 son los dos valores originales
                            MinimumThresholdForTheAcceptanceOfThePhrase = 0.0,
                            TheDocumentRepresentation = _docRep,
                            TheTFIDFWeight = _weight
                        },
                        MaximumNumberOfFitnessFunctionEvaluations = 1600,
                        TheFitnessFunction = FitnessFunction.MASDS,
                        TheFinalOrderOfSummary = FinalOrderOfSummary.MASDS,
                        N = 13,
                        L = 1,
                        M = 20,
                        Tenure = 8,
                        Alfa = 0.19,
                        Beta = 0.05,
                        Gamma = 0.06,
                        Delta = 0.05,
                        Ro = 0.65
                    };
                    break;
                case "SFLA":
                    mySummaryParameters = new SFLAParameters
                    {
                        DetailedReport = false,
                        MySummaryType = SummaryType.Words,
                        MaximumLengthOfSummaryForRouge = 100,
                        MyTDMParameters = new TDMParameters
                        {
                            MinimumFrequencyThresholdOfTermsForPhrase = 0, // 0 y 0 son los dos valores originales
                            MinimumThresholdForTheAcceptanceOfThePhrase = 0.0,
                            TheDocumentRepresentation = _docRep,
                            TheTFIDFWeight = _weight
                        },
                        MaximumNumberOfFitnessFunctionEvaluations = 1600,
                        TheFitnessFunction = FitnessFunction.MASDS,
                        TheFinalOrderOfSummary = FinalOrderOfSummary.MASDS,
                        M = 20,
                        C = 1,
                        Tenure = 8,
                        PondSize = 20,
                        NumberOfMemeplexes = 5,
                        MaxLocalIterations = 10,
                        ProbabilityOfMutation = 0.06,
                        Alfa = 0.15,
                        Beta = 0.04,
                        Gamma = 0.09,
                        Delta = 0.07,
                        Ro = 0.65
                    };
                    break;
            }
            var generator = new TestSummarizers();
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