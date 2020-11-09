using System.Collections.Generic;
using BusinessRules.VectorSpaceModel;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS
{
    public class GBHS: BaseAlgorithm
    {
        public List<Harmony> HarmonyMemory;

        public int OportunidadSA;

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            MyParameters = (GBHSParameters)mySummaryParameters;

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
            var myParameters = (GBHSParameters) MyParameters;

            CalculateRankingPhrasePosition();
            SortLengths();

            HarmonyMemory = new List<Harmony>();

            // Population initialization, fitness is calculated for each agent and local search is applied
            for (var i = 0; i < myParameters.HMS; i++)
            {
                var newAgent = new Harmony(this);
                newAgent.RandomInitialization();

                var improved = newAgent.Optimize();
                if (improved != null) newAgent = improved;

                HarmonyMemory.Add(newAgent);
                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations)  // MaxFFEs exceeded?
                    break;
            }

            // Sort the population from highest to lowest fitness ... it is maximizing
            HarmonyMemory.Sort((x,y) => -1 * x.Fitness.CompareTo(y.Fitness));

            while (CurrentFFEs < MaximumNumberOfFitnessFunctionEvaluations)
            {
                var miPAR = ParGn(myParameters.ParMin, myParameters.ParMax, CurrentFFEs, MaximumNumberOfFitnessFunctionEvaluations);
                var nuevoImproviso = new Harmony(this);
                var maxTries = HarmonyMemory[0].SelectedPhrases.Count;
                var triesCounter = 0;
                while (nuevoImproviso.SummaryLength < MyParameters.MaximumLengthOfSummaryForRouge)
                {
                    int pos;
                    if (MyParameters.RandomGenerator.NextDouble() < myParameters.HMCR)
                    {                                
                        var posEnMemoria = MyParameters.RandomGenerator.Next(myParameters.HMS);
                        if (MyParameters.RandomGenerator.NextDouble() < miPAR)
                            posEnMemoria = 0; // Choose the best harmony from harmony memory

                        var posFrase = MyParameters.RandomGenerator.Next(HarmonyMemory[posEnMemoria].SelectedPhrases.Count);
                        pos = HarmonyMemory[posEnMemoria].SelectedPhrases[posFrase];
                    }
                    else
                    {
                        pos = MyParameters.RandomGenerator.Next(SolutionSize);
                    }
                    nuevoImproviso.Activate(pos);

                    if (triesCounter++ > maxTries) break; // avoid long loop
                }

                nuevoImproviso.AddValidPhrases();
                nuevoImproviso.CalculateFitness();

                var improved = nuevoImproviso.Optimize();
                if (improved != null) nuevoImproviso = improved;

                if (!HarmonyMemory.Exists(x => x.Equals(nuevoImproviso)))
                    if (nuevoImproviso.Fitness > HarmonyMemory[HarmonyMemory.Count - 1].Fitness) // New harmony is better than the worst in the Harmony Memory?
                    {
                        HarmonyMemory.RemoveAt(HarmonyMemory.Count - 1);
                        HarmonyMemory.Add(nuevoImproviso);
                        HarmonyMemory.Sort((x, y) => -1*x.Fitness.CompareTo(y.Fitness));
                    }

                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations)  // MaxFFEs exceeded?
                    break;
            }

            HarmonyMemory[0].CompleteSummaryBasedOnCoverage();

            var phrasesList = new List<PositionValue>();
            var total = HarmonyMemory[0].SelectedPhrases.Count;
            for (var i = 0; i < total ; i++)
                phrasesList.Add(new PositionValue(HarmonyMemory[0].SelectedPhrases[i], 
                    total-i));

            return phrasesList;
        }

        private static double ParGn(double parMin, double parMax, int numCiclo, int ni)
        {
            var parGn = parMin + ((parMax - parMin) * (numCiclo * 1.0 / ni));
            return parGn;
        }
    }
}