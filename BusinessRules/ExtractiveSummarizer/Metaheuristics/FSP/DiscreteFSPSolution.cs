using System;
using System.Collections.Generic;
using System.Diagnostics;
using BusinessRules.Utils;

namespace BusinessRules.ExtractiveSummarizer.Metaheuristics.FSP
{
    public class DiscreteFSPSolution : BaseSolucion
    {   
        public DiscreteFSPSolution(AlgorithmBase myContainer): base(myContainer){
        }

        public DiscreteFSPSolution(BaseSolucion copia): base (copia){
        }

        public DiscreteFSPSolution ThrowFishingNet(int c)
        {
            try
            {
                var myParameters = (DiscreteFSPParameters)((DiscreteFSP)MyContainer).MyParameters;
                var m = myParameters.M;
                var candidateSolutionsGenerated = new List<DiscreteFSPSolution>();

                for (var i = 0; i < m; i++)
                {
                    var newCandidateSolution = new DiscreteFSPSolution(this);
                    var selectedPhrases = newCandidateSolution.ObtainPhrasesSortedByCoverageCosine();
                    var possiblePhrases = newCandidateSolution.MyContainer.ViablePhrases;
                    // Remove the last C sentences
                    if (selectedPhrases.Count > c)
                        selectedPhrases.RemoveRange(selectedPhrases.Count - c, c);
                    // Add C phrases to complete the solution
                    for (var j = 0; j < c; j++)
                    {
                        int idCandidatePhrase;
                        int candidatePosition;
                        bool noMorePossiblePhrases;
                        do
                        {
                            candidatePosition = myParameters.NumeroAleatorio.Next(possiblePhrases.Count);
                            idCandidatePhrase = possiblePhrases[candidatePosition].Position;
                            noMorePossiblePhrases = selectedPhrases.Count == possiblePhrases.Count;
                        } while (ActivePhrases.Contains(idCandidatePhrase) & noMorePossiblePhrases);
                        if (noMorePossiblePhrases) break;
                        selectedPhrases.Add(new PositionValue(idCandidatePhrase, possiblePhrases[candidatePosition].Value));
                    }

                    newCandidateSolution.ActivePhrases.Clear();
                    var totalWords = 0; // To control the size of the summary                 
                    while (totalWords < myParameters.MaximumLengthOfSummaryForRouge)
                    {
                        if (selectedPhrases.Count == 0) break;
                        var position = myParameters.NumeroAleatorio.Next(selectedPhrases.Count);
                        var value = selectedPhrases[position].Position;
                        totalWords += MyContainer.MyTDM.PhrasesList[value].Length;
                        selectedPhrases.RemoveAt(position);
                        newCandidateSolution.ActivePhrases.Add(value);
                    }
                    newCandidateSolution.ActivePhrases.Sort((x,y) => x.CompareTo(y));
                    newCandidateSolution.SummaryLength = totalWords;
                    newCandidateSolution.OptimizationComplete();
                    newCandidateSolution.CalculateFitness();

                    candidateSolutionsGenerated.Add(newCandidateSolution);
                }
                candidateSolutionsGenerated.Sort((x, y) => -1 * x.Fitness.CompareTo(y.Fitness));

                return candidateSolutionsGenerated[0].Fitness > Fitness ? candidateSolutionsGenerated[0] : new DiscreteFSPSolution(this);
            }
            catch (Exception e)
            {                
                Debug.WriteLine("Index error caught in ThrowFishingNet : " + e.Message);
                return null;
            }
        }
    }
}