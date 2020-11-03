using System.Collections.Generic;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;
using System.Linq;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.FSP
{
    public class DiscreteFSP: AlgorithmBase
    {
        public DiscreteFSPSolution Best;        
        public List<DiscreteFSPSolution> CapturePoints;
        public List<DiscreteFSPSolution> Pi; //  Local memory of the best solution found in each neighborhood of the capture point

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            MyParameters = (DiscreteFSPParameters)mySummaryParameters;

            MyTDM = new TDM(newsDirectory, MyParameters.MyTDMParameters, cacheFileName);
            MyExternalMDS = new SimilarityMatrix(MyTDM, cacheFileName, true);
            SolutionSize = MyTDM.PhrasesList.Count;

            var phrasesList = Execute();

            TextSummary = Util.SummarizeByCompressionRatio(MyTDM, phrasesList, mySummaryParameters.MySummaryType, 
                MyParameters.MaximumLengthOfSummaryForRouge, out SummaryByPhrases);
        }
        
        public List<KeyValuePair<int, double>> Execute()
        {
            CurrentFFEs = 0;
            MaximumNumberOfFitnessFunctionEvaluations = MyParameters.MaximumNumberOfFitnessFunctionEvaluations;
            var myParameters = (DiscreteFSPParameters) MyParameters;

            CalculateRankingPhrasePosition();
            ObtainViablePhrasesSortedByCosineCoverage();
            CapturePoints = new List<DiscreteFSPSolution>();
            Pi = new List<DiscreteFSPSolution>();
            Best = new DiscreteFSPSolution(this);

            // Population initialization, fitness is calculated for each agent and local search is applied
            for (var i = 0; i < myParameters.N; i++)
            {
                DiscreteFSPSolution newCapturePoint;
                do
                {
                    newCapturePoint = new DiscreteFSPSolution(this);
                    newCapturePoint.InicializarAleatorio();                    
                } while (CapturePoints.Exists(x => x.Equals(newCapturePoint)));                
                CapturePoints.Add(newCapturePoint);
                Pi.Add(newCapturePoint);
            }
            CapturePoints.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            Pi.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            Best = new DiscreteFSPSolution(CapturePoints[0]);
            
            var c = myParameters.C;
            for (var actualGeneration = 0; actualGeneration < myParameters.T; actualGeneration++)
            {
                for (var i = 0; i < myParameters.N; i++)
                {
                    // Each fisherman at his capture throws the net L times
                    var auxL = myParameters.L;
                    while ((auxL > 0))
                    {
                        var mejorsolucionLocal = CapturePoints[i].ThrowFishingNet(c); // Throw the network, update PI and Best if it is the case
                        if (mejorsolucionLocal.Fitness > Pi[i].Fitness)
                            Pi[i] = new DiscreteFSPSolution(mejorsolucionLocal);
                        auxL--;
                        if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
                    }
                    if (Pi[i].Fitness > CapturePoints[i].Fitness)
                        CapturePoints[i] = new DiscreteFSPSolution(Pi[i]);

                    if (CapturePoints[i].Fitness > Best.Fitness)
                        Best = new DiscreteFSPSolution(CapturePoints[i]);

                    if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
                }

                CapturePoints.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
                Pi.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
                if (CapturePoints[0].Fitness > Best.Fitness)
                    Best = new DiscreteFSPSolution(CapturePoints[0]);

                if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) break;
            }

            var phrasesList = SelectPhrasesFromFinalSummary(Best.ActivePhrases); 
            return phrasesList;
        }

        public override string ToString()
        {
            return CapturePoints.Aggregate("", (current, sol) => current + sol);
        }
    }
}