using System.Collections.Generic;
using System.Linq;
using BusinessRules.Utils;
using BusinessRules.VectorSpaceModel;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteSFLA
{   
    public class SFLA: BaseAlgorithm
    {
        public int FrogsByMemeplex;

        public Frog GBest;
        public List<Frog> Pond;
        public List<Frog>[] Memeplex;
        public TabuMemory MyTabuMemory;

        public override void Summarize(SummaryParameters mySummaryParameters, string newsDirectory, string cacheFileName)
        {
            MyParameters = (SFLAParameters)mySummaryParameters;
            MyTDM = new TDM(newsDirectory, MyParameters.MyTDMParameters, cacheFileName);
            MyExternalMDS = new SimilarityMatrix(MyTDM, cacheFileName);
            SolutionSize = MyTDM.PhrasesList.Count;

            MyTabuMemory = null;
            var phrasesList = Execute();

            TextSummary = Util.SummarizeByCompressionRatio(MyTDM, phrasesList, mySummaryParameters.MySummaryType,
                MyParameters.MaximumLengthOfSummaryForRouge, out SummaryByPhrases);
        }
        
        public List<PositionValue> Execute()
        {
            CurrentFFEs = 0;
            var myParameters = (SFLAParameters)MyParameters;
            
            // Setup of Memeplexes
            Memeplex = new List<Frog>[myParameters.NumberOfMemeplexes];
            for (var i = 0; i < myParameters.NumberOfMemeplexes; i++)
                Memeplex[i] = new List<Frog>();
            FrogsByMemeplex = myParameters.PondSize / myParameters.NumberOfMemeplexes;

            // Calculate the ranking of the position of the phrases
            CalculateRankingPhrasePosition();

            // Tabu memory of the specified type is created with the desired amount
            MyTabuMemory = new TabuMemory(myParameters.Tenure);

            // Initialize the Pond with frogs
            Pond = new List<Frog>();
            while (Pond.Count < myParameters.PondSize)
            {
                var frog = new Frog(this);
                frog.RandomInitialization();
                if (MyTabuMemory.IsTabu(frog.SelectedPhrases)) continue;
                Pond.Add(frog);
                MyTabuMemory.Include(frog.SelectedPhrases);
            }

            Pond.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            GBest = new Frog(Pond[0]);

            while (CurrentFFEs < MaximumNumberOfFitnessFunctionEvaluations)
            {
                ShuffletheFrogs();
                LocalSearch();
                RegroupTheFrogs();
                MutateTheFrogs();

                Pond.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
                GBest = new Frog(Pond[0]);
            }
            var mostRepeated = SelectToCompleteSummary(
                new List<BaseSolution>(Pond), GBest);
            var listaFrases = SelectPhrasesFromFinalSummary(
                GBest.SelectedPhrases, mostRepeated);  
            return listaFrases;
        }

        private void ShuffletheFrogs()
        {
            var i = 0;
            while (Pond.Count > 0)
            {
                var j = i % ((SFLAParameters)MyParameters).NumberOfMemeplexes;
                Memeplex[j].Add(Pond[0]);
                Pond.RemoveAt(0);
                i++;
            }
        }

        private void LocalSearch()
        {
            var myParameters = (SFLAParameters)MyParameters;
            for (var mplex = 0; mplex < myParameters.NumberOfMemeplexes; mplex++)
            {
                if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations) break;

                for (var iteration = 0; iteration < myParameters.MaxLocalIterations; iteration++)
                {
                    if (CurrentFFEs > MaximumNumberOfFitnessFunctionEvaluations) break;

                    var bestFrog = new Frog((Memeplex[mplex])[0]);
                    var worstFrog = new Frog((Memeplex[mplex])[FrogsByMemeplex - 1]);

                    var auxiliarFrog = new Frog(worstFrog);
                    auxiliarFrog.Jump(bestFrog);
                    if (MyTabuMemory.IsTabu(auxiliarFrog.SelectedPhrases)) auxiliarFrog.Fitness = 0;

                    if (auxiliarFrog.Fitness > worstFrog.Fitness)
                        (Memeplex[mplex])[FrogsByMemeplex - 1] = auxiliarFrog;
                    else
                    {
                        auxiliarFrog = new Frog(worstFrog);
                        auxiliarFrog.Jump(GBest);
                        if (MyTabuMemory.IsTabu(auxiliarFrog.SelectedPhrases)) auxiliarFrog.Fitness = 0;

                        if (auxiliarFrog.Fitness > worstFrog.Fitness)
                            (Memeplex[mplex])[FrogsByMemeplex - 1] = auxiliarFrog;
                        else
                        {
                            auxiliarFrog = new Frog(this);
                            auxiliarFrog.RandomInitialization();
                            if (!MyTabuMemory.IsTabu(auxiliarFrog.SelectedPhrases))
                                (Memeplex[mplex])[FrogsByMemeplex - 1] = auxiliarFrog;
                        }
                    }
                    if (auxiliarFrog.Fitness > GBest.Fitness)
                        GBest = new Frog(auxiliarFrog);

                }
                Memeplex[mplex].Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            }
        }

        private void RegroupTheFrogs()
        {
            for (var i = 0; i < ((SFLAParameters)MyParameters).NumberOfMemeplexes; i++)
            {
                Pond.AddRange(Memeplex[i]);
                Memeplex[i].Clear();
            }
        }

        private void MutateTheFrogs()
        {
            for (var i = 1; i < Pond.Count; i++)
            {
                var randomNumber = MyParameters.RandomGenerator.NextDouble();
                if (!(randomNumber < ((SFLAParameters)MyParameters).ProbabilityOfMutation)) continue;

                if (CurrentFFEs >= MaximumNumberOfFitnessFunctionEvaluations) return;
                var mutada = new Frog(Pond[i]);
                mutada.Mutate();
                Pond[i] = new Frog(mutada);
            }
        }

        public override string ToString()
        {
            return Pond.Aggregate("", (current, sol) => current + sol);
        }
    }
}