using System.Collections.Generic;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.GBHS
{
    public class Harmony : BaseSolution
    {
        public Harmony(BaseAlgorithm myContainer): base (myContainer){
        }

        public Harmony(BaseSolution origin): base(origin) {
        }

        /// <summary>
        /// Optimize removing bad phrases (based on coverage) and including randomly selected phrases
        /// </summary>
        /// <returns></returns>
        public Harmony Optimize()
        {
            // If it is NOT within the probability, it is NOT optimized
            if (((GBHSParameters)MyContainer.MyParameters).RandomGenerator.NextDouble() > 
                ((GBHSParameters)MyContainer.MyParameters).OptimizacionProbability)
                return null;

            return GredyOptimizationForBetterCoverage(1);
        }

        /// <summary>
        /// Create a list of MaxNumberOfOptimizacions of neigbors base on 
        /// removing bad phrases (based on coverage) and including randomly selected phrases,
        /// then return the best solution if is better tha the current one
        /// </summary>
        /// <param name="v">// number of phrases to be removed</param>
        private Harmony GredyOptimizationForBetterCoverage(int v)
        {
            var myParameters = (GBHSParameters) ((GBHS) MyContainer).MyParameters;

            var copyCurrentSolution = new Harmony(this);
            var selectedPhrases = copyCurrentSolution.ObtainSelectedPhrasesSortedByCoverageCosine();
            // Remove the last V sentences (phrases with low coverage) or total phrases if there are few selected ones
            var maxV = selectedPhrases.Count > v ? v : selectedPhrases.Count / 2;
            for (var i = 0; i < maxV; i++)
            {
                copyCurrentSolution.InActivate(selectedPhrases[selectedPhrases.Count - 1].Position);
                selectedPhrases.RemoveAt(selectedPhrases.Count - 1);
            }

            var candidateSolutionsGenerated = new List<Harmony>();
            for (var cambio = 0; cambio < myParameters.MaxNumberOfOptimizacions; cambio++)
            {
                var newCandidateSolution = new Harmony(copyCurrentSolution);
                // Try to add C or more phrases to complete the solution
                newCandidateSolution.AddValidPhrases(new List<int>());
                newCandidateSolution.CalculateFitness();
                candidateSolutionsGenerated.Add(newCandidateSolution);
            }
            candidateSolutionsGenerated.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));
            return candidateSolutionsGenerated[0].Fitness > Fitness? candidateSolutionsGenerated[0]: null;
        }
    }
}