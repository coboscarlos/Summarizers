using System.Collections.Generic;
using System.Linq;
using BusinessRules.Utils;
using BusinessRules.VectorSpaceModel;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteFSP
{
    public class FSP: BaseAlgorithm
    {
        public FSPSolution Best;        
        public List<FSPSolution> CapturePoints;
        public List<FSPSolution> Pi; //  Local memory of the best solution found in each neighborhood of the capture point

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            MyParameters = (FSPParameters)mySummaryParameters;

            MyTDM = new TDM(newsDirectory, MyParameters.MyTDMParameters, cacheFileName);
            MyExternalMDS = new SimilarityMatrix(MyTDM, cacheFileName);
            SolutionSize = MyTDM.PhrasesList.Count;

            var phrasesList = Execute();

            TextSummary = Util.SummarizeByCompressionRatio(MyTDM, phrasesList, mySummaryParameters.MySummaryType, 
                MyParameters.MaximumLengthOfSummaryForRouge, out SummaryByPhrases);
        }
        
        public List<PositionValue> Execute()
        {
            CurrentFFEs = 0;
            MaximumNumberOfFitnessFunctionEvaluations = MyParameters.MaximumNumberOfFitnessFunctionEvaluations;
            var myParameters = (FSPParameters) MyParameters;

            CalculateRankingPhrasePosition();
            SortLengths();

            CapturePoints = new List<FSPSolution>();
            Pi = new List<FSPSolution>();
            Best = new FSPSolution(this);

            // Population initialization, fitness is calculated for each agent and local search is applied
            while  (CapturePoints.Count < myParameters.N)
            {
                var newCapturePoint = new FSPSolution(this);
                newCapturePoint.RandomInitialization();
                if (CapturePoints.Exists(x => x.Equals(newCapturePoint))) continue;

                CapturePoints.Add(newCapturePoint);
                Pi.Add(newCapturePoint);
                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations)  // MaxFFEs exceeded?
                    break;
            }

            CapturePoints.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            Pi.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            Best = new FSPSolution(CapturePoints[0]);
            
            var c = myParameters.C;
            for (var actualGeneration = 0; actualGeneration < myParameters.T; actualGeneration++)
            {
                for (var i = 0; i < myParameters.N; i++)
                {
                    // Each fisherman at his capture throws the net L times
                    var auxL = 0;
                    while ((auxL < myParameters.L))
                    {
                        var mejorsolucionLocal = CapturePoints[i].ThrowFishingNet(c); // Throw the network, update PI and Best if it is the case
                        if (mejorsolucionLocal.Fitness > Pi[i].Fitness)
                            Pi[i] = new FSPSolution(mejorsolucionLocal);
                        auxL++;
                        if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
                    }
                    if (Pi[i].Fitness > CapturePoints[i].Fitness)
                        CapturePoints[i] = new FSPSolution(Pi[i]);

                    if (CapturePoints[i].Fitness > Best.Fitness)
                        Best = new FSPSolution(CapturePoints[i]);

                    if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
                }

                CapturePoints.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
                Pi.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));

                if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
            }

            var phrasesList = SelectPhrasesFromFinalSummary(Best.SelectedPhrases); 
            return phrasesList;
        }

        public override string ToString()
        {
            return CapturePoints.Aggregate("", (current, sol) => current + sol);
        }
    }
}