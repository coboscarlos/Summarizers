using System.Collections.Generic;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteFSP
{
    public class FSPSolution : BaseSolucion
    {   
        public FSPSolution(BaseAlgorithm myContainer): base(myContainer){
        }

        public FSPSolution(BaseSolucion origin): base (origin){
        }

        public FSPSolution ThrowFishingNet(int c)
        {
            var copyCurrentSolution = new FSPSolution(this);
            var selectedPhrases = copyCurrentSolution.ObtainSelectedPhrasesSortedByCoverageCosine();
            // Remove the last C sentences (phrases with low coverage) or total phrases if there are few selected ones
            var maxC = selectedPhrases.Count > c ? c : selectedPhrases.Count / 2;
                for (var j = 0; j < maxC; j++)
                {
                    copyCurrentSolution.InActivate(selectedPhrases[selectedPhrases.Count - 1].Position);
                    selectedPhrases.RemoveAt(selectedPhrases.Count - 1);
                }

            var candidateSolutionsGenerated = new List<FSPSolution>();
            for (var i = 0; i < ((FSPParameters)((FSP)MyContainer).MyParameters).M; i++)
            {
                var newCandidateSolution = new FSPSolution(copyCurrentSolution);
                // Try to add C or more phrases to complete the solution
                newCandidateSolution.AddValidPhrases();
                newCandidateSolution.CalculateFitness();
                candidateSolutionsGenerated.Add(newCandidateSolution);
            }
            candidateSolutionsGenerated.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));

            return candidateSolutionsGenerated[0].Fitness > Fitness ? candidateSolutionsGenerated[0] : new FSPSolution(this);
        }
    }
}