using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
                var myPAR = ParGn(myParameters.ParMin, myParameters.ParMax, CurrentFFEs, MaximumNumberOfFitnessFunctionEvaluations);
                var newimprovisation = new Harmony(this);
                var maxTries = 2 * HarmonyMemory[0].SelectedPhrases.Count;
                if (maxTries > SolutionSize) maxTries = SolutionSize - 1;
                var triesCounter = 0;
                while (newimprovisation.SummaryLength < MyParameters.MaximumLengthOfSummaryForRouge)
                {
                    int pos;
                    if (MyParameters.RandomGenerator.NextDouble() < myParameters.HMCR)
                    {                                
                        var posEnMemoria = MyParameters.RandomGenerator.Next(myParameters.HMS);
                        if (MyParameters.RandomGenerator.NextDouble() < myPAR)
                            posEnMemoria = 0; // Choose the best harmony from harmony memory

                        var posFrase = MyParameters.RandomGenerator.Next(HarmonyMemory[posEnMemoria].SelectedPhrases.Count);
                        pos = HarmonyMemory[posEnMemoria].SelectedPhrases[posFrase];
                    }
                    else
                    {
                        pos = MyParameters.RandomGenerator.Next(SolutionSize);
                    }
                    if (newimprovisation.SummaryLength + MyTDM.PhrasesList[pos].Length <=
                        MyParameters.MaximumLengthOfSummaryForRouge)
                        newimprovisation.Activate(pos);

                    if (triesCounter++ > maxTries) break; // avoid long loop
                }

                newimprovisation.AddValidPhrases();
                newimprovisation.CalculateFitness();

                var improved = newimprovisation.Optimize();
                if (improved != null) newimprovisation = improved;

                if (!HarmonyMemory.Exists(x => x.Equals(newimprovisation)))
                    if (newimprovisation.Fitness > HarmonyMemory[HarmonyMemory.Count - 1].Fitness) // New harmony is better than the worst in the Harmony Memory?
                    {
                        HarmonyMemory.RemoveAt(HarmonyMemory.Count - 1);
                        HarmonyMemory.Add(newimprovisation);
                        HarmonyMemory.Sort((x, y) => -1*x.Fitness.CompareTo(y.Fitness));
                    }

                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations)  // MaxFFEs exceeded?
                    break;
            }

            var mostRepeated = SelectToCompleteSummary();
            
            var phrasesList = SelectPhrasesFromFinalSummary(HarmonyMemory[0].SelectedPhrases, mostRepeated);
            return phrasesList;
        }

        private int SelectToCompleteSummary()
        {
            var listPhrases = new List<PositionValue>();
            foreach (var t in HarmonyMemory)
            {
                foreach (var phrase in t.SelectedPhrases)
                {
                    if (listPhrases.Exists(x => x.Position == phrase))
                    {
                        var currentPhrase = listPhrases.Find(x => x.Position == phrase);
                        currentPhrase.Value = currentPhrase.Value + 1;
                    }
                    else
                    {
                        var newPhrase = new PositionValue(phrase, 1);
                        listPhrases.Add(newPhrase);
                    }
                }
            }
            listPhrases.Sort((x,y) => -1 * x.Value.CompareTo(y.Value));
            foreach (var phrase in HarmonyMemory[0].SelectedPhrases)
            {
                if (listPhrases.Exists(x => x.Position == phrase))
                {
                    var currentPhrase = listPhrases.Find(x => x.Position == phrase);
                    listPhrases.Remove(currentPhrase);
                }
            }

            return listPhrases[0].Position;
        }

        private static double ParGn(double parMin, double parMax, int numCiclo, int ni)
        {
            var parGn = parMin + ((parMax - parMin) * (numCiclo * 1.0 / ni));
            return parGn;
        }
    }
}