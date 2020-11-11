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
            var pi = new List<FSPSolution>(); //  Local memory of the best solution found in each neighborhood of the capture point
            var lemt = new List<TabuMemory>();
            Best = new FSPSolution(this);

            // Population initialization, fitness is calculated for each agent and local search is applied
            for  (var i= 0; i < myParameters.N; i++)
            {
                var newCapturePoint = new FSPSolution(this);
                newCapturePoint.RandomInitialization();

                CapturePoints.Add(newCapturePoint);
                pi.Add(new FSPSolution(newCapturePoint));

                lemt.Add(new TabuMemory(myParameters.Tenure));
                lemt[lemt.Count-1].Include(newCapturePoint.SelectedPhrases);

                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations)  // MaxFFEs exceeded?
                    break;
            }

            CapturePoints.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            pi.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            Best = new FSPSolution(CapturePoints[0]);
            
            while (CurrentFFEs < MaximumNumberOfFitnessFunctionEvaluations)
            {
                for (var i = 0; i < myParameters.N; i++)
                {
                    // Each fisherman at his capture throws the net L times
                    for (var auxL = 0; auxL < myParameters.L; auxL++)
                    {
                        // Throw the network, update PI and Best if it is the case
                        var mejorsolucionLocal = 
                            CapturePoints[i].GenerateNeighbor(lemt[i]);
                        lemt[i].Include(mejorsolucionLocal.SelectedPhrases);

                        if (mejorsolucionLocal.Fitness > pi[i].Fitness)
                            pi[i] = new FSPSolution(mejorsolucionLocal);
                        if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
                    }
                    if (pi[i].Fitness > CapturePoints[i].Fitness)
                        CapturePoints[i] = new FSPSolution(pi[i]);

                    if (CapturePoints[i].Fitness > Best.Fitness)
                        Best = new FSPSolution(CapturePoints[i]);

                    if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
                }

                CapturePoints.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
                pi.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));

                if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
            }

            var mostRepeated = SelectToCompleteSummary(new List<BaseSolution>(CapturePoints), Best);
            var phrasesList = SelectPhrasesFromFinalSummary(Best.SelectedPhrases, mostRepeated);

            return phrasesList;
        }

        public override string ToString()
        {
            return CapturePoints.Aggregate("", (current, sol) => current + sol);
        }
    }
}