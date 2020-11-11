using System.Collections.Generic;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.DiscreteFSP
{
    public class FSPSolution : BaseSolution
    {   
        public FSPSolution(BaseAlgorithm myContainer): base(myContainer){
        }

        public FSPSolution(BaseSolution origin): base (origin){
        }

        public FSPSolution GenerateNeighbor(TabuMemory explicitLocalTabuMemory)
        {
            var copyCurrentSolution = new FSPSolution(this);
            var selectedPhrases = copyCurrentSolution.ObtainSelectedPhrasesSortedByCoverageCosine();
            // Remove the last sentence (phrases with low coverage) ... C = 1
            copyCurrentSolution.InActivate(selectedPhrases[selectedPhrases.Count - 1].Position);
            var excludePhrases = new List<int> {selectedPhrases[selectedPhrases.Count - 1].Position};

            var tries = 0;
            const int maxTries = 5;
            FSPSolution newCandidateSolution = null;
            do
            {
                newCandidateSolution = new FSPSolution(copyCurrentSolution);
                // Try to add one or more randomly selected phrases to complete the solution
                newCandidateSolution.AddValidPhrases(excludePhrases);
                if (tries++ < maxTries) break; // avoid long time in the loop
            } while (explicitLocalTabuMemory.IsTabu(newCandidateSolution.SelectedPhrases));
            newCandidateSolution.CalculateFitness();

            return newCandidateSolution;
        }
    }
}